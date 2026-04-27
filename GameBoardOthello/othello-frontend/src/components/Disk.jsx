function Disk({ color, small }) {
    const cls = color?.toLowerCase()
    return (
        <div className={`disk disk--${cls}${small ? ' disk--sm' : ''}`}>
            <div className="disk__shine" />
        </div>
    )
}

export default Disk