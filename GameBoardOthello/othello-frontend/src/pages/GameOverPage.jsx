// src/pages/GameOverPage.jsx
import Board from '../components/Board'
import Disk  from '../components/Disk'

function GameOverPage({ gameState, onPlayAgain, onHome }) {
    // No completed game yet — show empty state
    if (!gameState) {
        return (
            <div className="page page--gameover">
                <div className="gameover-left">
                    <h1 className="gameover-headline">Game<br /><em>Over.</em></h1>
                    <div className="gameover-empty">
                        <p className="gameover-empty__icon">🎱</p>
                        <p className="gameover-empty__title">No completed game yet</p>
                        <p className="gameover-empty__body">
                            Start a match from the Home page and finish it to see results here.
                        </p>
                    </div>
                    <div className="gameover-actions">
                        <button className="btn btn--light" onClick={onHome}>Back to Home</button>
                    </div>
                </div>
                <div className="gameover-right gameover-right--empty">
                    <p className="gameover-right-hint">
                        Results will appear here after a completed match.
                    </p>
                </div>
            </div>
        )
    }

    const { winner, score, player1, player2, board } = gameState
    const blackScore = score?.blackScore ?? 0
    const whiteScore = score?.whiteScore ?? 0
    const margin     = Math.abs(blackScore - whiteScore)
    const isDraw     = !winner

    return (
        <div className="page page--gameover">
            <div className="gameover-left">
                <h1 className="gameover-headline">Game<br /><em>Over.</em></h1>

                <div className="winner-card">
                    {isDraw ? (
                        <>
                            <div className="winner-card__disks">
                                <Disk color="Black" /><Disk color="White" />
                            </div>
                            <div>
                                <p className="winner-card__label">Result</p>
                                <p className="winner-card__name">Draw</p>
                                <p className="winner-card__margin">Equal stones</p>
                            </div>
                        </>
                    ) : (
                        <>
                            <Disk color={winner.color} />
                            <div>
                                <p className="winner-card__label">Winner</p>
                                <p className="winner-card__name">{winner.name}</p>
                                <p className="winner-card__margin">
                                    Wins by {margin} stone{margin !== 1 ? 's' : ''}
                                </p>
                            </div>
                        </>
                    )}
                </div>

                <div className="final-score">
                    <div className="final-score__side">
                        <Disk color="Black" small />
                        <span className="final-score__num">{blackScore}</span>
                        <span className="final-score__name">{player1?.name}</span>
                    </div>
                    <span className="final-score__vs">vs</span>
                    <div className="final-score__side final-score__side--right">
                        <span className="final-score__num">{whiteScore}</span>
                        <Disk color="White" small />
                        <span className="final-score__name">{player2?.name}</span>
                    </div>
                </div>

                <div className="gameover-actions">
                    <button className="btn btn--light"   onClick={onPlayAgain}>Play Again</button>
                    <button className="btn btn--outline" onClick={onHome}>Back to Home</button>
                </div>
            </div>

            <div className="gameover-right">
                <div className="board-card board-card--dim">
                    <Board boardData={board} onSquareClick={() => {}} disabled />
                </div>
                <div className="gameover-breakdown">
                    <div>
                        <p className="breakdown-label">Black</p>
                        <p className="breakdown-val">{blackScore}</p>
                    </div>
                    <div>
                        <p className="breakdown-label">White</p>
                        <p className="breakdown-val">{whiteScore}</p>
                    </div>
                    <div>
                        <p className="breakdown-label">Empty</p>
                        <p className="breakdown-val">{64 - blackScore - whiteScore}</p>
                    </div>
                </div>
            </div>
        </div>
    )
}

export default GameOverPage