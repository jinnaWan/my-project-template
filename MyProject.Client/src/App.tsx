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
        <h1>My Project Template</h1>
      </header>
      
      <main className="app-content">
        <div className="card mb-4">
          <button onClick={() => setCount((count) => count + 1)}>
            count is {count}
          </button>
        </div>
        
        <TodoList />
      </main>
      
      <footer className="app-footer">
        <p>© {new Date().getFullYear()} MyProject - MVVM Example</p>
      </footer>
    </div>
  )
}

export default App
