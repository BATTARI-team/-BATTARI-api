import { useState } from 'react'
import './App.css'
import { Header } from './Header'
import { BrowserRouter } from 'react-router-dom'
import { AppRoutes } from './Routes'
import { TokenContext, TokenProvider } from './provider/TokenProvider'
import { Battari } from './Battari'

function App() {
  const [count, setCount] = useState(0)

  return (
    <BrowserRouter>
      <TokenProvider>
        <Battari>
          <Header/>
          <AppRoutes/>
        </Battari>
      </TokenProvider>
    </BrowserRouter>
  )
}

export default App
