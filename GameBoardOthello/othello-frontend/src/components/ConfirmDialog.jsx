// src/components/ConfirmDialog.jsx

function ConfirmDialog({ message, onConfirm, onCancel }) {
    return (
        <div className="confirm-overlay" onClick={onCancel}>
            <div className="confirm-dialog" onClick={e => e.stopPropagation()}>
                <p className="confirm-msg">{message}</p>
                <div className="confirm-actions">
                    <button className="btn btn--danger" onClick={onConfirm}>
                        Yes, Exit
                    </button>
                    <button className="btn btn--outline" onClick={onCancel}>
                        Cancel — Stay in Game
                    </button>
                </div>
            </div>
        </div>
    )
}

export default ConfirmDialog