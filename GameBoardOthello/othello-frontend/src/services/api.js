// src/services/api.js
import axios from 'axios'

const BASE_URL =
    import.meta.env.VITE_API_BASE_URL || 'http://localhost:5143/api'

const http = axios.create({
    baseURL: BASE_URL,
    headers: { 'Content-Type': 'application/json' },
    timeout: 8000,
})

const gameAPI = {
    // POST /game/start
    startGame: async (player1Name, player2Name) => {
        const res = await http.post('/game/start', { player1Name, player2Name })
        return res.data
    },

    // GET /game/{id}/state
    getState: async (gameId) => {
        const res = await http.get(`/game/${gameId}/state`)
        return res.data
    },

    // POST /game/{id}/move
    makeMove: async (gameId, row, col) => {
        const res = await http.post(`/game/${gameId}/move`, { row, col })
        return res.data
    },

    // GET /game/{id}/valid-moves
    getValidMoves: async (gameId) => {
        const res = await http.get(`/game/${gameId}/valid-moves`)
        return res.data
    },

    // POST /game/{id}/skip-turn
    skipTurn: async (gameId) => {
        const res = await http.post(`/game/${gameId}/skip-turn`)
        return res.data
    },

    // DELETE /game/{id}
    deleteGame: async (gameId) => {
        const res = await http.delete(`/game/${gameId}`)
        return res.data
    },
}

export default gameAPI