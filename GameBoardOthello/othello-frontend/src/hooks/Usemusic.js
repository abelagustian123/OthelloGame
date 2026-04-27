// src/hooks/useMusic.js
import { useState, useEffect, useRef, useCallback } from 'react'

const SFX_MAP = {
    start: '/music/GameStart.mp3',
    move:  '/music/moveDisk.mp3',
}

export function useMusic(bgmSrc) {
    const [musicOn, setMusicOn] = useState(false)
    const audioRef = useRef(null)

    useEffect(() => {
        const audio = new Audio(bgmSrc)
        audio.loop   = true
        audio.volume = 0.4
        audioRef.current = audio

        return () => {
            audio.pause()
            audio.src = ''
        }
    }, [bgmSrc])

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

    const playSfx = useCallback((name) => {
        const src = SFX_MAP[name]
        if (!src) return

        const sfx = new Audio(src)
        sfx.volume = name === 'start' ? 0.6 : 0.5
        sfx.play().catch(() => {})
    }, [])

    return { musicOn, toggleMusic, playSfx }
}