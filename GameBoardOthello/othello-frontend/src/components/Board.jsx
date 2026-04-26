// src/components/Board.jsx
import Disk from './Disk'

const COLS = ['A','B','C','D','E','F','G','H']

// Square is internal to Board — not exported, used only here
function Square({ data, onClick, disabled }) {
    const { row, col, disk, isValidMove, isLastMove } = data
    const handleClick = () => { if (isValidMove && !disabled) onClick(row, col) }

    return (
        <div
            className={[
                'sq',
                isValidMove && !disabled ? 'sq--valid' : '',
                isLastMove ? 'sq--last' : '',
            ].filter(Boolean).join(' ')}
            onClick={handleClick}
        >
            {disk
                ? <Disk color={disk.color} />
                : (isValidMove && !disabled)
                    ? <div className="sq__hint" />
                    : null}
        </div>
    )
}

function Board({ boardData, onSquareClick, disabled }) {
    if (!boardData?.length) return null

    return (
        <div className="board-shell">
            <div className="board-col-labels">
                {COLS.map(c => <span key={c}>{c}</span>)}
            </div>
            <div className="board-rows">
                {boardData.map((rowArr, ri) => (
                    <div key={ri} className="board-row">
                        <span className="board-row-label">{ri + 1}</span>
                        {rowArr.map(sq => (
                            <Square
                                key={`${sq.row}-${sq.col}`}
                                data={sq}
                                onClick={onSquareClick}
                                disabled={disabled}
                            />
                        ))}
                    </div>
                ))}
            </div>
        </div>
    )
}

export default Board