import { Route, Routes } from "react-router-dom";
import { Login } from "./Login"; // Import the Login component from its source file
import { Index } from "./Index";

export function AppRoutes() {
    return (
        <Routes>
            <Route path="/login" element={<Login/>} />
            <Route path="/" element={<Index/>} />
        </Routes>
    )
}