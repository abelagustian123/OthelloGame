import { useState, useCallback } from 'react'

const SKIP_SHOW_MS     = 2500   
const SKIP_FADEOUT_MS  = 400   

const GAMEEND_SHOW_MS    = 3000
const GAMEEND_FADEOUT_MS = 400

export function useNotifications(setPage) {
    const [skipNotif,    setSkipNotif]    = useState(null)
    const [gameEndNotif, setGameEndNotif] = useState(null)

    const showSkipNotif = useCallback((skippedPlayer, nextPlayer) => {
        return new Promise((resolve) => {
            setSkipNotif({ skippedPlayer, nextPlayer, isExiting: false })

            setTimeout(() => {
                setSkipNotif(prev => prev ? { ...prev, isExiting: true } : null)

                setTimeout(() => {
                    setSkipNotif(null)
                    resolve()
                }, SKIP_FADEOUT_MS)

            }, SKIP_SHOW_MS)
        })
    }, [])

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