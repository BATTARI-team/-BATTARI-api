import React from "react";

interface TokenProviderContext {
    token: string;
    setToken: (token: string) => void;
}

export interface IThemeProviderProps {
    children: React.ReactNode;
}

export const TokenContext = React.createContext<TokenProviderContext>({
    token: "",
    setToken: () => {},
});

export const TokenProvider: React.FC<IThemeProviderProps> = ({children}) => {
    const [token, _setToken] = React.useState('');

    function setToken(token: string) {
        _setToken(token);
    }

    return (
        <TokenContext.Provider value={{token, setToken}}>
            {children}
        </TokenContext.Provider>
    );
}