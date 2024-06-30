import { useState } from 'react'
import './App.css'
import { Header } from './Header'
import { BrowserRouter } from 'react-router-dom'
import { AppRoutes } from './Routes'

function App() {
  const [count, setCount] = useState(0)

  return (
    <BrowserRouter>
      <Header/>

      <AppRoutes/>
      

    </BrowserRouter>
  )
}

export default App
