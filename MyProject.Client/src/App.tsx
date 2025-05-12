import { useState } from 'react'
import './App.css'
import { TodoList } from './components/TodoList'

/**
 * Main application component
 */
function App() {
  const [count, setCount] = useState(0)

  return (
    <div className="app-container">
      <header className="app-header">
        <h1>Todo App</h1>
        <p className="app-subtitle">Simple, modern task management</p>
      </header>
      
      <main className="app-content">
        <TodoList />
        
        <div className="card my-4">
          <p>Click counter demo:</p>
          <button onClick={() => setCount((count) => count + 1)}>
            Clicks: {count}
          </button>
        </div>
      </main>
      
      <footer className="app-footer">
        <p>© {new Date().getFullYear()} Todo App - Built with React</p>
      </footer>
    </div>
  )
}

export default App
