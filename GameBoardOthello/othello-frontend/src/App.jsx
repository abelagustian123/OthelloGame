import { useState, useCallback, useEffect } from 'react'
import gameAPI from './services/api'

import { hasAnyValidMove, applyDemoMove, countDisks } from './utils/boardUtils'
import { useMusic }          from './hooks/useMusic'
import { useNotifications }  from './hooks/useNotifications'
import { useDemoScenarios }  from './hooks/useDemoScenarios'

import Nav                  from './components/Nav'
import ConfirmDialog        from './components/ConfirmDialog'
import SkipNotification     from './components/SkipNotification'
import GameEndNotification  from './components/GameEndNotification'

import HomePage             from './pages/HomePage'
import GamePage             from './pages/GamePage'
import GameOverPage         from './pages/GameOverPage'

import './styles/Game.css'

export default function App() {

  const [page, setPage] = useState('Home')  

  const [gameId,            setGameId]            = useState(null)
  const [gameState,         setGameState]         = useState(null)
  const [lastFinishedState, setLastFinishedState] = useState(null)

  const [p1, setP1] = useState('')
  const [p2, setP2] = useState('')

  const [loading,     setLoading]     = useState(false)
  const [moveLoading, setMoveLoading] = useState(false)
  const [skipLoading, setSkipLoading] = useState(false)
  const [statusMsg,   setStatusMsg]   = useState('')

  const [confirmTarget, setConfirmTarget] = useState(null) // null | 'home' | 'gameover'
  const [isDemoMode,    setIsDemoMode]    = useState(false)

  const { musicOn, toggleMusic, playSfx } = useMusic('/music/musicAssets.mp3')

  const {
    skipNotif, showSkipNotif,
    gameEndNotif, showGameEnd, goToGameOverNow,
  } = useNotifications(setPage)

  const { handleDemoEndGame, handleDemoOnePass, handleDemoBothPass } =
      useDemoScenarios({
        gameState, setGameState, setIsDemoMode, setStatusMsg,
        showSkipNotif, showGameEnd, setLastFinishedState, setPage,
      })

  const gameInProgress =
      gameId !== null && gameState !== null && !gameState.isGameOver

  useEffect(() => {
    const fn = (e) => {
      if (e.key === 'Escape') {
        if (gameInProgress) { setConfirmTarget('home') }
        else { doGoHome() }
      }
    }
    window.addEventListener('keydown', fn)
    return () => window.removeEventListener('keydown', fn)
  }, [gameInProgress])

  const doGoHome = () => {
    setPage('Home'); setGameId(null); setGameState(null)
    setStatusMsg(''); setP1(''); setP2('')
    setConfirmTarget(null); setIsDemoMode(false)
  }

  const handleNavigate = (target) => {
    if (target === page) return

    if (target === 'Home') {
      if (gameInProgress) { setConfirmTarget('home') }
      else { doGoHome() }
      return
    }

    if (target === 'Game') {
      setPage('Game')
      return
    }

    if (target === 'Game Over') {
      if (gameInProgress) { setConfirmTarget('gameover') }
      else { setPage('Game Over') }
      return
    }
  }

  const handleConfirmYes = () => {
    if (confirmTarget === 'home') {
      doGoHome()
    }
    if (confirmTarget === 'gameover') {
      setGameId(null)
      setGameState(null)
      setIsDemoMode(false)
      setStatusMsg('')
      setPage('Game Over')
    }
    setConfirmTarget(null)
  }

  const handleConfirmNo = () => setConfirmTarget(null)

  const handleStart = useCallback(async () => {
    const name1 = p1.trim() || 'Player 1'
    const name2 = p2.trim() || 'Player 2'
    setLoading(true)
    const data = await gameAPI.startGame(name1, name2)
    setLoading(false)
    if (data?.gameId) {
      setGameId(data.gameId); setGameState(data)
      setStatusMsg(''); setIsDemoMode(false); setPage('Game')
      playSfx('start')
    }
  }, [p1, p2, playSfx])

  const doSkip = useCallback(async (stateBeforeSkip, gId) => {
    setSkipLoading(true)
    const skippedPlayer = stateBeforeSkip?.currentPlayer
    setStatusMsg(`${skippedPlayer?.name ?? 'Player'} has no moves — skipping…`)
    await new Promise(r => setTimeout(r, 600))

    const data = await gameAPI.skipTurn(gId)
    setSkipLoading(false)
    if (!data) { setStatusMsg('Skip failed'); return }

    showSkipNotif(skippedPlayer, data.currentPlayer)
    setGameState(data); setStatusMsg('')

    if (data.isGameOver) {
      setLastFinishedState(data)
      showGameEnd(data)
      return
    }
    if (!hasAnyValidMove(data.board)) { await doSkip(data, gId) }
  }, [showSkipNotif, showGameEnd])

  const handleRealMove = useCallback(async (row, col) => {
    if (!gameId || moveLoading || skipLoading) return
    setMoveLoading(true); setStatusMsg('')

    const data = await gameAPI.makeMove(gameId, row, col)
    setMoveLoading(false)

    if (!data?.success) {
      setStatusMsg(data?.errorMessage || 'Invalid move')
      return
    }

    const newState = data.gameState
    setGameState(newState)
    playSfx('move')

    if (newState.isGameOver) {
      setLastFinishedState(newState)
      showGameEnd(newState)
      return
    }
    if (!hasAnyValidMove(newState.board)) { await doSkip(newState, gameId) }
  }, [gameId, moveLoading, skipLoading, doSkip, showGameEnd, playSfx])

  const handleDemoMove = useCallback((row, col) => {
    if (!gameState || moveLoading) return
    const playerColor = gameState.currentPlayer?.color
    if (!playerColor) return

    const newBoard  = applyDemoMove(gameState.board, row, col, playerColor)
    const { black, white } = countDisks(newBoard)
    playSfx('move')
    const emptyCount = newBoard.flat().filter(sq => !sq.disk).length

    if (emptyCount === 0) {
      const winner =
          black > white ? gameState.player1 :
              white > black ? gameState.player2 : null
      const finalState = {
        ...gameState,
        board: newBoard,
        score: { blackScore: black, whiteScore: white },
        isGameOver: true,
        winner,
      }
      setGameState(finalState)
      setLastFinishedState(finalState)
      showGameEnd(finalState)
      return
    }

    const nextPlayer =
        playerColor === 'Black' ? gameState.player2 : gameState.player1

    setGameState(prev => ({
      ...prev,
      board: newBoard,
      currentPlayer: nextPlayer,
      score: { blackScore: black, whiteScore: white },
    }))
    setStatusMsg(`${nextPlayer.name} to Move`)
  }, [gameState, moveLoading, showGameEnd, playSfx])

  const handleMove = isDemoMode ? handleDemoMove : handleRealMove

  const handlePlayAgain = () => {
    setPage('Home'); setGameId(null); setGameState(null)
    setStatusMsg(''); setIsDemoMode(false)
  }

  const handleBackToHome = () => {
    setPage('Home'); setGameId(null); setGameState(null)
    setStatusMsg(''); setP1(''); setP2(''); setIsDemoMode(false)
  }

  const statusDot =
      page === 'Home'      ? 'ready' :
          page === 'Game'      ? 'live'  : 'concluded'

  const gameOverState =
      page === 'Game Over' && gameState?.isGameOver
          ? gameState
          : lastFinishedState

  return (
      <div className="app">

        <Nav
            page={page}
            onNavigate={handleNavigate}
            statusDot={statusDot}
            musicOn={musicOn}
            onToggleMusic={toggleMusic}
            gameInProgress={gameInProgress}
        />

        {page === 'Home' && (
            <HomePage
                p1={p1} setP1={setP1}
                p2={p2} setP2={setP2}
                onStart={handleStart}
                loading={loading}
            />
        )}

        {page === 'Game' && (
            <GamePage
                gameState={gameState}
                onMove={handleMove}
                moveLoading={moveLoading}
                skipLoading={skipLoading}
                statusMsg={statusMsg}
                gameId={gameId}
                isDemoMode={isDemoMode}
                onDemoEndGame={handleDemoEndGame}
                onDemoOnePass={handleDemoOnePass}
                onDemoBothPass={handleDemoBothPass}
            />
        )}

        {page === 'Game Over' && (
            <GameOverPage
                gameState={gameOverState}
                onPlayAgain={handlePlayAgain}
                onHome={handleBackToHome}
            />
        )}

        {confirmTarget && (
            <ConfirmDialog
                message={
                  confirmTarget === 'home'
                      ? 'Are you sure you want to exit? The current game will be lost.'
                      : 'Are you sure you want to leave? The current game will be lost.'
                }
                onConfirm={handleConfirmYes}
                onCancel={handleConfirmNo}
            />
        )}

        {gameEndNotif && (
            <GameEndNotification
                winner={gameEndNotif.winner}
                score={gameEndNotif.score}
                player1={gameEndNotif.player1}
                player2={gameEndNotif.player2}
                isExiting={gameEndNotif.isExiting}
                onGoNow={goToGameOverNow}
            />
        )}

        {skipNotif && (
            <SkipNotification
                skippedPlayer={skipNotif.skippedPlayer}
                nextPlayer={skipNotif.nextPlayer}
                isExiting={skipNotif.isExiting}
            />
        )}

      </div>
  )
}