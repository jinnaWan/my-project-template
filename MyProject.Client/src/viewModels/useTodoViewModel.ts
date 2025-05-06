import { useState, useEffect, useCallback } from 'react';
import { Todo } from '../models/Todo';
import { TodoService } from '../services/apiService';

/**
 * View model hook for managing Todo items
 */
export function useTodoViewModel() {
  const [todos, setTodos] = useState<Todo[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [newTodoTitle, setNewTodoTitle] = useState('');

  /**
   * Fetches all Todo items from the API
   */
  const fetchTodos = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await TodoService.getAllTodos();
      setTodos(data);
    } catch (err) {
      setError('Failed to fetch todos. Make sure the API is running.');
      console.error('Error in fetchTodos:', err);
    } finally {
      setLoading(false);
    }
  }, []);

  /**
   * Creates a new Todo item
   */
  const addTodo = useCallback(async () => {
    if (!newTodoTitle.trim()) return;

    try {
      setError(null);
      const newTodo = await TodoService.createTodo({
        title: newTodoTitle,
        isCompleted: false
      });
      
      setTodos(prevTodos => [...prevTodos, newTodo]);
      setNewTodoTitle('');
    } catch (err) {
      setError('Failed to add todo');
      console.error('Error in addTodo:', err);
    }
  }, [newTodoTitle]);

  /**
   * Toggles the completion status of a Todo item
   * @param id - The identifier of the Todo item to toggle
   */
  const toggleTodoCompletion = useCallback(async (id: number) => {
    try {
      setError(null);
      const todoToUpdate = todos.find(todo => todo.id === id);
      
      if (!todoToUpdate) return;
      
      const updatedTodo = { 
        ...todoToUpdate, 
        isCompleted: !todoToUpdate.isCompleted 
      };
      
      await TodoService.updateTodo(updatedTodo);
      
      setTodos(prevTodos => 
        prevTodos.map(todo => 
          todo.id === id ? updatedTodo : todo
        )
      );
    } catch (err) {
      setError(`Failed to update todo #${id}`);
      console.error('Error in toggleTodoCompletion:', err);
    }
  }, [todos]);

  /**
   * Deletes a Todo item
   * @param id - The identifier of the Todo item to delete
   */
  const deleteTodo = useCallback(async (id: number) => {
    try {
      setError(null);
      await TodoService.deleteTodo(id);
      setTodos(prevTodos => prevTodos.filter(todo => todo.id !== id));
    } catch (err) {
      setError(`Failed to delete todo #${id}`);
      console.error('Error in deleteTodo:', err);
    }
  }, []);

  // Load todos when the component mounts
  useEffect(() => {
    fetchTodos();
  }, [fetchTodos]);

  return {
    todos,
    loading,
    error,
    newTodoTitle,
    setNewTodoTitle,
    addTodo,
    toggleTodoCompletion,
    deleteTodo,
    fetchTodos
  };
} 