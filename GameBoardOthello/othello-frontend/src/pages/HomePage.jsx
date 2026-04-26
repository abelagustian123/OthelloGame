// src/pages/HomePage.jsx
import Disk from '../components/Disk'

function HomePage({ p1, setP1, p2, setP2, onStart, loading }) {
    const handleKey = (e) => { if (e.key === 'Enter') onStart() }

    return (
        <div className="page page--home">
            <div className="home-left">
                <p className="home-eyebrow">— A Game of Reversal · Est. 1883</p>
                <h1 className="home-headline">
                    Black<br />against<br /><em>White.</em>
                </h1>
                <p className="home-body">
                    Two players. Sixty-four squares. One simple rule of capture.
                    Outflank, flip, and finish with the most stones on the board.
                    A minute to learn — a lifetime to refine.
                </p>
                <div className="home-divider" />
                <div className="home-stats">
                    <div className="home-stat">
                        <span className="home-stat__label">Players</span>
                        <span className="home-stat__value">2</span>
                    </div>
                    <div className="home-stat">
                        <span className="home-stat__label">Avg. Duration</span>
                        <span className="home-stat__value">12 min</span>
                    </div>
                    <div className="home-stat">
                        <span className="home-stat__label">Board</span>
                        <span className="home-stat__value">8 × 8</span>
                    </div>
                </div>
            </div>

            <div className="home-right">
                <div className="match-card">
                    <div className="match-card__header">
                        <span className="match-card__title">New Match</span>
                        <span className="match-card__live">● Live</span>
                    </div>

                    <div className="match-input-row">
                        <Disk color="Black" small />
                        <div className="match-input-wrap">
                            <span className="match-input-label">Player 01 · Black · Moves First</span>
                            <input
                                className="match-input"
                                placeholder="Enter name…"
                                value={p1}
                                onChange={e => setP1(e.target.value)}
                                onKeyDown={handleKey}
                                autoFocus
                                maxLength={20}
                            />
                        </div>
                    </div>

                    <div className="match-input-row">
                        <Disk color="White" small />
                        <div className="match-input-wrap">
                            <span className="match-input-label">Player 02 · White</span>
                            <input
                                className="match-input"
                                placeholder="Enter name…"
                                value={p2}
                                onChange={e => setP2(e.target.value)}
                                onKeyDown={handleKey}
                                maxLength={20}
                            />
                        </div>
                    </div>

                    <button className="btn btn--start" onClick={onStart} disabled={loading}>
                        {loading ? 'Starting…' : 'Start Game →'}
                    </button>

                    <div className="match-card__shortcuts">
                        <kbd>Enter</kbd>&nbsp;start&nbsp;&nbsp;
                        <kbd>Esc</kbd>&nbsp;home
                    </div>
                </div>
            </div>
        </div>
    )
}

export default HomePage