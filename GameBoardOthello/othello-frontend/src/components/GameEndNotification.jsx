const CONFETTI_COLORS = ['#d4b483', '#e8e4dc', '#6aab72', '#e05555', '#7ec8e3']

function GameEndNotification({ winner, score, player1, player2, isExiting, onGoNow }) {
    const isDraw      = !winner
    const blackScore  = score?.blackScore ?? 0
    const whiteScore  = score?.whiteScore ?? 0
    const margin      = Math.abs(blackScore - whiteScore)
    const winnerColor = winner?.color?.toLowerCase()

    const confettiDots = !isDraw
        ? Array.from({ length: 18 }, (_, i) => ({
            id: i,
            left:  `${Math.random() * 100}%`,
            color: CONFETTI_COLORS[i % CONFETTI_COLORS.length],
            delay: `${Math.random() * 0.8}s`,
            dur:   `${1.2 + Math.random() * 1.2}s`,
        }))
        : []

    return (
        <div className={`gameend-overlay${isExiting ? ' gameend-overlay--out' : ''}`}>
            <div className={`gameend-card${isExiting ? ' gameend-card--out' : ''}`}>

                <div className="gameend-card__stripe" />

                {!isDraw && (
                    <div className="gameend-confetti">
                        {confettiDots.map(d => (
                            <div
                                key={d.id}
                                className="confetti-dot"
                                style={{
                                    left: d.left,
                                    background: d.color,
                                    animationDelay: d.delay,
                                    animationDuration: d.dur,
                                }}
                            />
                        ))}
                    </div>
                )}

                <p className="gameend-card__eyebrow">Game Over</p>

                <div className="gameend-card__disks">
                    {isDraw ? (
                        <>
                            <div className="disk disk--black"><div className="disk__shine" /></div>
                            <div className="disk disk--white"><div className="disk__shine" /></div>
                        </>
                    ) : (
                        <div className={`disk disk--${winnerColor}`}>
                            <div className="disk__shine" />
                        </div>
                    )}
                </div>

                <div>
                    {isDraw ? (
                        <h2 className="gameend-card__headline">It's a <em>Draw!</em></h2>
                    ) : (
                        <h2 className="gameend-card__headline"><em>{winner.name}</em> Wins!</h2>
                    )}
                    {!isDraw && (
                        <p className="gameend-card__margin">
                            Wins by {margin} stone{margin !== 1 ? 's' : ''}
                        </p>
                    )}
                </div>

                <div className="gameend-card__score">
                    <div className="gameend-score-side">
                        <div className="disk disk--black disk--sm"><div className="disk__shine" /></div>
                        <span className="gameend-score-num">{blackScore}</span>
                        <span className="gameend-score-name">{player1?.name}</span>
                    </div>
                    <span className="gameend-score-vs">vs</span>
                    <div className="gameend-score-side">
                        <span className="gameend-score-name">{player2?.name}</span>
                        <span className="gameend-score-num">{whiteScore}</span>
                        <div className="disk disk--white disk--sm"><div className="disk__shine" /></div>
                    </div>
                </div>

                <div className="gameend-card__footer">
                    <p className="gameend-card__hint">Redirecting to results…</p>
                    <div className="gameend-card__bar">
                        <div className="gameend-card__bar-fill" />
                    </div>
                    <button className="btn btn--start" onClick={onGoNow}>
                        See Results →
                    </button>
                </div>

            </div>
        </div>
    )
}

export default GameEndNotification