import './App.css'
import { Header } from './Header'
import { BrowserRouter } from 'react-router-dom'
import { AppRoutes } from './Routes'
import { TokenProvider } from './provider/TokenProvider'
import { Battari } from './Battari'

function App() {
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
