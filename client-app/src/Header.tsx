import { Link } from 'react-router-dom';
import './Header.css';
export function Header() {
    return (
        <header className="header">
            <nav>
                <ul className="ul">
                    <li><Link to="/login">Login</Link></li>
                    <li>Register</li>
                </ul>
            </nav>
        </header>
    );
}