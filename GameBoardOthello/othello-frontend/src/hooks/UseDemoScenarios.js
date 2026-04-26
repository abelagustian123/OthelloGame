// src/hooks/useDemoScenarios.js
import { useCallback } from 'react'
import { buildBoard, countDisks } from '../utils/boardUtils'

export function useDemoScenarios({
                                     gameState,
                                     setGameState,
                                     setIsDemoMode,
                                     setStatusMsg,
                                     showSkipNotif,
                                     showGameEnd,
                                     setLastFinishedState,
                                     setPage,
                                 }) {

    // ── Demo 1: Near Game End ──────────────────────────────────────
    const handleDemoEndGame = useCallback(() => {
        if (!gameState) return

        //     A  B  C  D  E  F  G  H
        // 1:  B  B  B  B  B  W  W  _   ← H1 empty (last move)
        // 2–5: all Black
        // 6–8: all White
        const colorMap = Array.from({ length: 8 }, (_, row) =>
            Array.from({ length: 8 }, (_, col) => {
                if (row === 0 && col === 7) return null
                if (row === 0 && col >= 5)  return 'White'
                if (row <= 4)               return 'Black'
                return 'White'
            })
        )

        const demoBoard = buildBoard(colorMap, [[0, 7]])
        const { black, white } = countDisks(demoBoard)

        setIsDemoMode(true)
        setGameState(prev => ({
            ...prev,
            board: demoBoard,
            currentPlayer: prev.player1,
            score: { blackScore: black, whiteScore: white },
            isGameOver: false,
        }))
        setStatusMsg('Demo: Black clicks H1 to end the game!')
    }, [gameState, setGameState, setIsDemoMode, setStatusMsg])

    // ── Demo 2: One Player Pass ────────────────────────────────────
    const handleDemoOnePass = useCallback(() => {
        if (!gameState) return

        //     A  B  C  D  E  F  G  H
        // 1:  .  .  .  _  .  .  .  .   ← D1 valid for White
        // 2:  .  .  .  B  .  .  .  .
        // 3:  .  .  .  B  .  .  .  .
        // 4:  .  W  B  W  B  W  .  .
        // 5:  .  .  .  B  .  .  .  .
        // 6:  .  .  .  B  .  .  .  .
        const colorMap = Array.from({ length: 8 }, () =>
                Array.from({ length: 8 }, () => null)
            )
        ;[[1,3],[2,3],[4,2],[4,4],[5,3],[6,3]].forEach(([r,c]) => { colorMap[r][c] = 'Black' })
        ;[[3,1],[3,3],[3,5]].forEach(([r,c]) => { colorMap[r][c] = 'White' })

        const p1 = gameState.player1
        const p2 = gameState.player2

        setIsDemoMode(true)
        setGameState(prev => ({
            ...prev,
            board: buildBoard(colorMap, []),
            currentPlayer: prev.player1,
            score: { blackScore: 6, whiteScore: 3 },
            isGameOver: false,
        }))
        setStatusMsg(`${p1?.name} (Black) has no valid moves!`)

        setTimeout(async () => {
            // Show Black skip notif, wait for it to finish, then switch to White
            await showSkipNotif(p1, p2)
            setGameState(prev => ({
                ...prev,
                board: buildBoard(colorMap, [[0, 3]]),
                currentPlayer: prev.player2,
                score: { blackScore: 6, whiteScore: 3 },
                isGameOver: false,
            }))
            setStatusMsg(`${p2?.name} (White) moves! Click D1.`)
            setTimeout(() => setStatusMsg(''), 3000)
        }, 1200)
    }, [gameState, setGameState, setIsDemoMode, setStatusMsg, showSkipNotif])

    // ── Demo 3: Both Players Pass ──────────────────────────────────
    // What user sees:
    //   1. Board shown, Black's turn, status "Black has no valid moves!"
    //   2. SkipNotif: "Black — no moves → White's turn"      (await, 2.9s)
    //   3. SkipNotif: "White — no moves → game ending ⚠"     (await, 2.9s)
    //   4. GameEnd notification → Game Over page
    const handleDemoBothPass = useCallback(() => {
        if (!gameState) return

        //     A  B  C  D  E  F  G  H
        // 1–4: all Black
        // 5–7: all White
        // 8:  W  W  _  W  W  _  W  W   ← 2 isolated empty squares
        const colorMap = Array.from({ length: 8 }, (_, row) =>
            Array.from({ length: 8 }, (_, col) => {
                if (row === 7 && col === 2) return null
                if (row === 7 && col === 5) return null
                if (row <= 3) return 'Black'
                return 'White'
            })
        )

        const boardStuck = buildBoard(colorMap, [])
        const { black, white } = countDisks(boardStuck)
        const p1 = gameState.player1
        const p2 = gameState.player2

        const winner =
            black > white ? p1 :
                white > black ? p2 : null

        const finalState = {
            ...gameState,
            board: boardStuck,
            isGameOver: true,
            score: { blackScore: black, whiteScore: white },
            winner,
        }

        // Show board with Black stuck
        setIsDemoMode(true)
        setGameState(prev => ({
            ...prev,
            board: boardStuck,
            currentPlayer: prev.player1,
            score: { blackScore: black, whiteScore: white },
            isGameOver: false,
        }))
        setStatusMsg(`${p1?.name} (Black) has no valid moves!`)

        // Run the full sequence asynchronously
        setTimeout(async () => {

            // Step 1: Black skip notification
            // "Black cannot move — White's turn next"
            setStatusMsg(`${p1?.name} (Black) is skipped!`)
            await showSkipNotif(p1, p2)

            // Step 2: Switch board to show White's turn
            setGameState(prev => ({
                ...prev,
                board: boardStuck,
                currentPlayer: p2,
                score: { blackScore: black, whiteScore: white },
                isGameOver: false,
            }))
            setStatusMsg(`${p2?.name} (White) also has no valid moves!`)

            // Small pause so user can read the status message
            await new Promise(r => setTimeout(r, 800))

            // Step 3: White skip notification
            // "White cannot move — game ending"
            setStatusMsg(`${p2?.name} (White) is skipped!`)
            await showSkipNotif(p2, null)

            // Step 4: Game over
            setStatusMsg('Neither player can move — Game Over!')
            setLastFinishedState(finalState)
            showGameEnd(finalState)

        }, 1200)

    }, [gameState, setGameState, setIsDemoMode, setStatusMsg,
        showSkipNotif, showGameEnd, setLastFinishedState, setPage])

    return { handleDemoEndGame, handleDemoOnePass, handleDemoBothPass }
}