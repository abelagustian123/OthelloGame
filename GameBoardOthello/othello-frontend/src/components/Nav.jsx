// src/components/Nav.jsx
import { useState, useCallback, useRef } from 'react'

// Tab accessibility rules:
// 'Game'      — only when game is in progress
// 'Game Over' — always (shows empty state if no finished game)
// 'Home'      — always
const TAB_MESSAGES = {
    Game: 'Start a game first to access this page.',
}

function Nav({ page, onNavigate, statusDot, musicOn, onToggleMusic, gameInProgress }) {
    const tabs = ['Home', 'Game', 'Game Over']
    const statusLabel =
        statusDot === 'ready'     ? 'Ready'    :
            statusDot === 'live'      ? 'In Match' : 'Concluded'

    const [toast, setToast] = useState(null)  // { message, isExiting }
    const toastTimerRef = useRef(null)

    const showToast = useCallback((message) => {
        // Clear any existing timer
        if (toastTimerRef.current) clearTimeout(toastTimerRef.current)

        setToast({ message, isExiting: false })

        // Start exit after 2s
        toastTimerRef.current = setTimeout(() => {
            setToast(prev => prev ? { ...prev, isExiting: true } : null)
            setTimeout(() => setToast(null), 250)
        }, 2000)
    }, [])

    const handleTabClick = useCallback((tab) => {
        // Tab is currently active — do nothing
        if (tab === page) return

        // 'Game' tab — only accessible when game is in progress
        if (tab === 'Game' && !gameInProgress) {
            showToast(TAB_MESSAGES.Game)
            return
        }

        // All other cases — delegate to parent
        onNavigate(tab)
    }, [page, gameInProgress, onNavigate, showToast])

    const isTabDisabled = (tab) => {
        if (tab === 'Game' && !gameInProgress) return true
        return false
    }

    return (
        <>
            <nav className="nav">
                <div className="nav-brand">
                    <div className="nav-brand__disk" />
                    <span>Othello</span>
                </div>

                <div className="nav-tabs">
                    {tabs.map(t => (
                        <button
                            key={t}
                            className={[
                                'nav-tab',
                                page === t          ? 'nav-tab--active'   : '',
                                isTabDisabled(t)    ? 'nav-tab--disabled'  : '',
                            ].filter(Boolean).join(' ')}
                            onClick={() => handleTabClick(t)}
                        >
                            {t}
                        </button>
                    ))}
                </div>

                <div className="nav-status">
                    <span className={`nav-dot nav-dot--${statusDot}`} />
                    <span>{statusLabel}</span>

                    {/* Music toggle */}
                    <button
                        className={`music-toggle ${musicOn ? 'music-toggle--on' : ''}`}
                        onClick={onToggleMusic}
                        title={musicOn ? 'Mute music' : 'Play music'}
                    >
                        {musicOn ? (
                            <>
                                <div className="music-bars">
                                    <div className="music-bars__bar" />
                                    <div className="music-bars__bar" />
                                    <div className="music-bars__bar" />
                                    <div className="music-bars__bar" />
                                </div>
                                <span>Music On</span>
                            </>
                        ) : (
                            <>
                                <span className="music-toggle__icon">♪</span>
                                <span>Music Off</span>
                            </>
                        )}
                    </button>
                </div>
            </nav>

            {/* Nav toast — appears below nav when tab is not accessible */}
            {toast && (
                <div className={`nav-toast${toast.isExiting ? ' nav-toast--out' : ''}`}>
                    {toast.message}
                </div>
            )}
        </>
    )
}

export default Nav