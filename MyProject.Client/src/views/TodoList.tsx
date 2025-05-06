import { useTodoViewModel } from '../viewModels/useTodoViewModel';
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
      
      {error && <div className="error">{error}</div>}
      
      <div className="add-todo">
        <input
          type="text"
          value={newTodoTitle}
          onChange={(e) => setNewTodoTitle(e.target.value)}
          placeholder="Add new todo"
          onKeyPress={handleKeyPress}
        />
        <button onClick={addTodo}>Add</button>
      </div>
      
      {loading ? (
        <p>Loading todos...</p>
      ) : (
        <ul className="todo-list">
          {todos.length === 0 ? (
            <li className="empty-list">No todos yet. Add one above!</li>
          ) : (
            todos.map((todo) => (
              <li key={todo.id} className={todo.isCompleted ? 'completed' : ''}>
                <span 
                  className="todo-title"
                  onClick={() => toggleTodoCompletion(todo.id)}
                >
                  {todo.title}
                </span>
                <button 
                  className="delete-btn" 
                  onClick={() => deleteTodo(todo.id)}
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