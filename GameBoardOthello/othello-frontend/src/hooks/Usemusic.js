// src/hooks/useMusic.js
import { useState, useEffect, useRef, useCallback } from 'react'

/**
 * Manages background music playback with on/off toggle.
 * Handles browser autoplay policy by waiting for first user interaction.
 *
 * @param {string} src - Path to the audio file (e.g. '/music/musicAssets.mp3')
 * @returns {{ musicOn: boolean, toggleMusic: () => void }}
 */
export function useMusic(src) {
    const [musicOn, setMusicOn] = useState(true)
    const audioRef = useRef(null)

    // Initialize audio once on mount
    useEffect(() => {
        const audio = new Audio(src)
        audio.loop   = true
        audio.volume = 0.4
        audioRef.current = audio

        // Browsers block autoplay until first user interaction.
        // Try immediately; if blocked, wait for the first click.
        const tryPlay = () => {
            audio.play().catch(() => {
                const unlock = () => {
                    audio.play().catch(() => {})
                    document.removeEventListener('click', unlock)
                }
                document.addEventListener('click', unlock)
            })
        }
        tryPlay()

        return () => {
            audio.pause()
            audio.src = ''
        }
    }, [src])

    // Sync play/pause whenever musicOn changes
    useEffect(() => {
        const audio = audioRef.current
        if (!audio) return
        if (musicOn) {
            audio.play().catch(() => {})
        } else {
            audio.pause()
        }
    }, [musicOn])

    const toggleMusic = useCallback(() => {
        setMusicOn(prev => !prev)
    }, [])

    return { musicOn, toggleMusic }
}