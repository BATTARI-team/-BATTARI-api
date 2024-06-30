import { Link } from 'react-router-dom';
import './Header.css';
import { useContext } from 'react';
import {Button} from '@mui/material';
import { TokenContext } from './provider/TokenProvider';

export function Header() {
    const {token, setToken} = useContext(TokenContext);
    
    return (
        <header className="header">
            <nav>
                <ul className="ul">
                    <li><Link to="/login">Login</Link></li>
                    <li>Register</li>
                </ul>
                <p>token: {token}</p>
                    <p>token: {token}</p>
                    <Button onClick={() => { setToken("testets");console.log("konnnitiha");}}>ログイン</Button>
            </nav>
        </header>
    );
}