export const hasAnyValidMove = (board) =>
    board?.some((row) => row.some((sq) => sq.isValidMove)) ?? false

export function buildBoard(colorMap, validMoves = []) {
    return Array.from({ length: 8 }, (_, row) =>
        Array.from({ length: 8 }, (_, col) => ({
            row,
            col,
            disk: colorMap[row]?.[col] ? { color: colorMap[row][col] } : null,
            isValidMove: validMoves.some(([r, c]) => r === row && c === col),
            isLastMove: false,
        }))
    )
}

export function applyDemoMove(board, row, col, playerColor) {
    const opponentColor = playerColor === 'Black' ? 'White' : 'Black'
    const DIRS = [[-1,-1],[-1,0],[-1,1],[0,-1],[0,1],[1,-1],[1,0],[1,1]]
    const rows = board.length
    const cols = board[0].length

    // Deep-copy board
    const next = board.map(r =>
        r.map(sq => ({ ...sq, disk: sq.disk ? { ...sq.disk } : null }))
    )

    // Place the new disk
    next[row][col] = {
        ...next[row][col],
        disk: { color: playerColor },
        isLastMove: true,
        isValidMove: false,
    }

    // Flip sandwiched disks in all 8 directions
    for (const [dr, dc] of DIRS) {
        const toFlip = []
        let r = row + dr
        let c = col + dc

        while (r >= 0 && r < rows && c >= 0 && c < cols) {
            const sq = next[r][c]
            if (!sq.disk) break
            if (sq.disk.color === opponentColor) {
                toFlip.push([r, c])
            } else {
                for (const [fr, fc] of toFlip) {
                    next[fr][fc] = { ...next[fr][fc], disk: { color: playerColor } }
                }
                break
            }
            r += dr
            c += dc
        }
    }

    // Clear all isValidMove / isLastMove flags except the placed square
    return next.map(r =>
        r.map(sq => ({
            ...sq,
            isValidMove: false,
            isLastMove: sq.row === row && sq.col === col,
        }))
    )
}

export function countDisks(board) {
    let black = 0
    let white = 0
    board.flat().forEach(sq => {
        if (sq.disk?.color === 'Black') black++
        else if (sq.disk?.color === 'White') white++
    })
    return { black, white }
}