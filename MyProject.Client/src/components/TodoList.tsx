import { useTodoViewModel } from '../hooks/useTodoViewModel';
import '../styles/TodoList.css';

/**
 * Component for displaying and managing a list of Todo items
 */
export function TodoList() {
  const {
    todos,
    loading,
    error,
    newTodoTitle,
    setNewTodoTitle,
    addTodo,
    toggleTodoCompletion,
    deleteTodo
  } = useTodoViewModel();

  const handleKeyPress = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') {
      addTodo();
    }
  };

  return (
    <div className="todo-container">
      <h2>Todo List</h2>
      
      {error && <div className="error" role="alert">{error}</div>}
      
      <div className="add-todo">
        <input
          type="text"
          value={newTodoTitle}
          onChange={(e) => setNewTodoTitle(e.target.value)}
          placeholder="What needs to be done?"
          onKeyPress={handleKeyPress}
          aria-label="New todo title"
        />
        <button 
          onClick={addTodo}
          disabled={!newTodoTitle.trim()}
          aria-label="Add todo"
        >
          Add
        </button>
      </div>
      
      {loading ? (
        <div className="loading-indicator">Loading todos...</div>
      ) : (
        <ul className="todo-list" role="list">
          {todos.length === 0 ? (
            <li className="empty-list">No todos yet. Add one above!</li>
          ) : (
            todos.map((todo) => (
              <li 
                key={todo.id} 
                className={todo.isCompleted ? 'completed' : ''}
              >
                <div 
                  className="todo-title"
                  onClick={() => toggleTodoCompletion(todo.id)}
                  onKeyDown={(e) => {
                    if (e.key === 'Enter' || e.key === ' ') {
                      e.preventDefault();
                      toggleTodoCompletion(todo.id);
                    }
                  }}
                  role="checkbox"
                  aria-checked={todo.isCompleted}
                  tabIndex={0}
                >
                  {todo.title}
                </div>
                <button 
                  className="delete-btn" 
                  onClick={() => deleteTodo(todo.id)}
                  aria-label={`Delete todo: ${todo.title}`}
                >
                  Delete
                </button>
              </li>
            ))
          )}
        </ul>
      )}
    </div>
  );
} 