import Board  from '../components/Board'
import Disk   from '../components/Disk'

const TIPS = [
    'Corners can never be flipped — secure them.',
    'Avoid X-squares (next to corners) early.',
    'Mobility beats material in the midgame.',
    'Edges form the spine of the endgame.',
]

function GamePage({
                      gameState,
                      onMove,
                      moveLoading,
                      skipLoading,
                      statusMsg,
                      gameId,
                      isDemoMode,
                      onDemoEndGame,
                      onDemoOnePass,
                      onDemoBothPass,
                  }) {
    if (!gameState) return null

    const { board, player1, player2, currentPlayer, score, isGameOver } = gameState
    const legalCount = board.flat().filter(sq => sq.isValidMove).length
    const disabled   = moveLoading || skipLoading || isGameOver
    const blackScore = score?.blackScore ?? 2
    const whiteScore = score?.whiteScore ?? 2

    // Demo buttons only when board is at initial state (4 disks, demo not active)
    const disksOnBoard = board.flat().filter(sq => sq.disk !== null).length
    const showDemo     = disksOnBoard === 4 && !isDemoMode

    return (
        <div className="page page--game">

            {/* ── Left sidebar ── */}
            <aside className="game-sidebar">

                <div className="sidebar-section">
                    <p className="sidebar-label">Players</p>
                    {[
                        { player: player1, color: 'Black', meta: 'Black · Plays First', sc: blackScore },
                        { player: player2, color: 'White', meta: 'White',               sc: whiteScore },
                    ].map(({ player, color, meta, sc }) => (
                        <div
                            key={color}
                            className={`player-row ${
                                currentPlayer?.color === color && !isGameOver ? 'player-row--active' : ''
                            }`}
                        >
                            <Disk color={color} small />
                            <div className="player-row__info">
                                <p className="player-row__name">{player?.name}</p>
                                <p className="player-row__meta">{meta}</p>
                            </div>
                            <span className="player-row__score">{sc}</span>
                        </div>
                    ))}
                </div>

                {showDemo && (
                    <div className="sidebar-section">
                        <p className="sidebar-label">
                            Demo Scenarios
                            <span className="demo-badge">Presentation Only</span>
                        </p>
                        <div className="demo-buttons">
                            <button className="btn btn--demo" onClick={onDemoEndGame}>
                                <span className="demo-btn-icon">⚡</span>
                                <span>
                  <strong>Near Game End</strong>
                  <small>Black places last piece to win</small>
                </span>
                            </button>
                            <button className="btn btn--demo" onClick={onDemoOnePass}>
                                <span className="demo-btn-icon">⏭</span>
                                <span>
                  <strong>One Player Pass</strong>
                  <small>Black can't move → White plays</small>
                </span>
                            </button>
                            <button className="btn btn--demo" onClick={onDemoBothPass}>
                                <span className="demo-btn-icon">🔚</span>
                                <span>
                  <strong>Both Players Pass</strong>
                  <small>Neither can move → Game Over</small>
                </span>
                            </button>
                        </div>
                    </div>
                )}

                <div className="sidebar-section sidebar-section--grow">
                    <p className="sidebar-label">How to Play</p>
                    <p className="sidebar-body">
                        Place a disc to outflank your opponent in any straight line —
                        horizontal, vertical, or diagonal. Captured discs flip to your
                        color. The match ends when neither player can move.
                        Most discs wins.
                    </p>
                </div>

            </aside>

            <main className="game-center">
                <div className="game-center__header">
                    <h2 className="game-center__title">
                        Match&nbsp;
                        <span className="game-center__id">
              #{gameId?.slice(0, 4).toUpperCase() ?? '----'}
            </span>
                    </h2>
                    {!isGameOver && (
                        <div className="turn-badge">
                            <Disk color={currentPlayer?.color} small />
                            <span>
                {skipLoading ? 'Switching turn…'
                    : statusMsg  ? statusMsg
                        : `${currentPlayer?.name} to Move`}
              </span>
                        </div>
                    )}
                </div>

                <div className="board-card">
                    <Board boardData={board} onSquareClick={onMove} disabled={disabled} />
                </div>
            </main>

            <aside className="game-panel">
                <div className="score-card">
                    <p className="sidebar-label">Score</p>
                    <div className="score-card__row">
                        {[
                            { player: player1, color: 'Black', sc: blackScore },
                            { player: player2, color: 'White', sc: whiteScore },
                        ].map(({ player, color, sc }) => (
                            <div key={color} className="score-card__item">
                                <Disk color={color} small />
                                <span className="score-card__num">{sc}</span>
                                <span className="score-card__name">{player?.name}</span>
                            </div>
                        ))}
                    </div>
                </div>

                <div className="tips-card">
                    <p className="sidebar-label">Strategy Tips</p>
                    <ol className="tips-list">
                        {TIPS.map((tip, i) => (
                            <li key={i}>
                                <span className="tips-num">0{i + 1}</span>
                                {tip}
                            </li>
                        ))}
                    </ol>
                </div>
            </aside>

            <div className="game-statusbar">
                <span>● {legalCount} Legal</span>
                <span>● Black {blackScore}</span>
                <span>● White {whiteScore}</span>
            </div>

        </div>
    )
}

export default GamePage