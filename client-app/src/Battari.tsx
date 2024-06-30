import React from "react";
import { TokenContext } from "./provider/TokenProvider";

type BattariProps = {
    children: React.ReactNode;
}

export function Battari({children}: BattariProps) {
    init();
    return (
        <>
            <div>
            {children}
            </div>
        </>
    )
}

async function init(): Promise<void>{
    const {token, setToken} = React.useContext(TokenContext);
    
    //ここでtokenを読み込む
    await new Promise(resolve => setTimeout(resolve, 1000));
    setToken("finished");
}