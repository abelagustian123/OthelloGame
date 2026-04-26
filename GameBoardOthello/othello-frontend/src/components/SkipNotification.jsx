// src/components/SkipNotification.jsx

function SkipNotification({ skippedPlayer, nextPlayer, isExiting }) {
    const skippedColor = skippedPlayer?.color?.toLowerCase()
    const nextColor    = nextPlayer?.color?.toLowerCase()

    return (
        <>
            {/* Backdrop blur */}
            <div
                className={`skip-notif__backdrop${isExiting ? ' skip-notif__backdrop--out' : ''}`}
            />

            {/* Card */}
            <div className={`skip-notif${isExiting ? ' skip-notif--out' : ''}`}>
                <div className="skip-notif__card">

                    {/* Skipped player */}
                    <div className="skip-notif__disk-row">
                        {skippedColor && (
                            <div className={`disk disk--${skippedColor} disk--sm`}>
                                <div className="disk__shine" />
                            </div>
                        )}
                        <div>
                            <p className="skip-notif__label">No valid moves</p>
                            <p className="skip-notif__player">{skippedPlayer?.name}</p>
                        </div>
                    </div>

                    <p className="skip-notif__reason">
                        Cannot place a disc — turn is skipped
                    </p>

                    {/* Next player — null means game is ending (both players stuck) */}
                    {nextPlayer ? (
                        <div className="skip-notif__next">
                            {nextColor && (
                                <div className={`disk disk--${nextColor} disk--sm`}>
                                    <div className="disk__shine" />
                                </div>
                            )}
                            <span>{nextPlayer.name}'s turn now</span>
                        </div>
                    ) : (
                        <div className="skip-notif__next skip-notif__next--gameover">
                            <span>⚠ Neither player can move — game ending</span>
                        </div>
                    )}

                    {/* Progress bar */}
                    <div className="skip-notif__progress">
                        <div className="skip-notif__progress-bar" />
                    </div>

                </div>
            </div>
        </>
    )
}

export default SkipNotification