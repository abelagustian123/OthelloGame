import axios from 'axios'

const BASE_URL =
    import.meta.env.VITE_API_BASE_URL || 'http://localhost:5143/api'

const http = axios.create({
    baseURL: BASE_URL,
    headers: { 'Content-Type': 'application/json' },
    timeout: 8000,
})

const gameAPI = {
    startGame: async (player1Name, player2Name) => {
        const res = await http.post('/game/start', { player1Name, player2Name })
        return res.data
    },

    getState: async (gameId) => {
        const res = await http.get(`/game/${gameId}/state`)
        return res.data
    },

    makeMove: async (gameId, row, col) => {
        const res = await http.post(`/game/${gameId}/move`, { row, col })
        return res.data
    },

    getValidMoves: async (gameId) => {
        const res = await http.get(`/game/${gameId}/valid-moves`)
        return res.data
    },

    skipTurn: async (gameId) => {
        const res = await http.post(`/game/${gameId}/skip-turn`)
        return res.data
    },

    deleteGame: async (gameId) => {
        const res = await http.delete(`/game/${gameId}`)
        return res.data
    },
}

export default gameAPI