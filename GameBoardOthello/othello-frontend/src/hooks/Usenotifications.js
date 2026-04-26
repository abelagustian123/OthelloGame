// src/hooks/useNotifications.js
import { useState, useCallback } from 'react'

// Durations
const SKIP_SHOW_MS     = 2500   // how long notification is visible
const SKIP_FADEOUT_MS  = 400    // fade-out animation

const GAMEEND_SHOW_MS    = 3000
const GAMEEND_FADEOUT_MS = 400

/**
 * showSkipNotif(player, nextPlayer) — show one skip notification.
 * Returns a Promise that resolves when the notification is fully gone.
 * This allows callers to await it and chain notifications sequentially.
 *
 * showGameEnd(finalState) — shows game-end notification then redirects.
 */
export function useNotifications(setPage) {
    const [skipNotif,    setSkipNotif]    = useState(null)
    const [gameEndNotif, setGameEndNotif] = useState(null)

    // ── showSkipNotif ───────────────────────────────────────────────
    // Returns a Promise so callers can chain:
    //   await showSkipNotif(p1, p2)
    //   await showSkipNotif(p2, null)
    //   showGameEnd(finalState)
    const showSkipNotif = useCallback((skippedPlayer, nextPlayer) => {
        return new Promise((resolve) => {
            // Show
            setSkipNotif({ skippedPlayer, nextPlayer, isExiting: false })

            // Start exit after display time
            setTimeout(() => {
                setSkipNotif(prev => prev ? { ...prev, isExiting: true } : null)

                // Remove from DOM, then resolve so next can start
                setTimeout(() => {
                    setSkipNotif(null)
                    resolve()
                }, SKIP_FADEOUT_MS)

            }, SKIP_SHOW_MS)
        })
    }, [])

    // ── showGameEnd ─────────────────────────────────────────────────
    const showGameEnd = useCallback((finalState) => {
        const { winner, score, player1, player2 } = finalState

        setGameEndNotif({ winner, score, player1, player2, isExiting: false })

        setTimeout(() => {
            setGameEndNotif(prev => prev ? { ...prev, isExiting: true } : null)
            setTimeout(() => {
                setGameEndNotif(null)
                setPage('Game Over')
            }, GAMEEND_FADEOUT_MS)
        }, GAMEEND_SHOW_MS)
    }, [setPage])

    // ── goToGameOverNow ─────────────────────────────────────────────
    const goToGameOverNow = useCallback(() => {
        setGameEndNotif(prev => prev ? { ...prev, isExiting: true } : null)
        setTimeout(() => {
            setGameEndNotif(null)
            setPage('Game Over')
        }, 350)
    }, [setPage])

    return {
        skipNotif,
        showSkipNotif,
        gameEndNotif,
        showGameEnd,
        goToGameOverNow,
    }
}