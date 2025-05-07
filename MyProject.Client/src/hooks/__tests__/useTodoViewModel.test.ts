import { describe, it, expect, vi, beforeEach } from 'vitest';
import { renderHook, act } from '@testing-library/react';
import { useTodoViewModel } from '../useTodoViewModel';
import { TodoService } from '../../services/apiService';
import { Todo } from '../../models/Todo';

// Mock the TodoService
vi.mock('../../services/apiService', () => ({
  TodoService: {
    getAllTodos: vi.fn(),
    getTodoById: vi.fn(),
    createTodo: vi.fn(),
    updateTodo: vi.fn(),
    deleteTodo: vi.fn()
  }
}));

describe('useTodoViewModel', () => {
  const mockTodos = [
    { id: 1, title: 'Test Todo 1', isCompleted: false },
    { id: 2, title: 'Test Todo 2', isCompleted: true }
  ];
  
  beforeEach(() => {
    vi.clearAllMocks();
    
    // Default mock implementations
    (TodoService.getAllTodos as unknown as ReturnType<typeof vi.fn>).mockResolvedValue(mockTodos);
    (TodoService.createTodo as unknown as ReturnType<typeof vi.fn>).mockImplementation((todo: Omit<Todo, 'id'>) => 
      Promise.resolve({ id: 3, ...todo }));
    (TodoService.updateTodo as unknown as ReturnType<typeof vi.fn>).mockResolvedValue(undefined);
    (TodoService.deleteTodo as unknown as ReturnType<typeof vi.fn>).mockResolvedValue(undefined);
  });
  
  it('should fetch todos on initialization', async () => {
    const { result } = renderHook(() => useTodoViewModel());
    
    // Initially loading should be true
    expect(result.current.loading).toBe(true);
    
    // Wait for the useEffect to complete
    await vi.waitFor(() => {
      expect(result.current.loading).toBe(false);
    });
    
    expect(TodoService.getAllTodos).toHaveBeenCalledTimes(1);
    expect(result.current.todos).toEqual(mockTodos);
    expect(result.current.error).toBeNull();
  });
  
  it('should handle fetch todos error', async () => {
    (TodoService.getAllTodos as unknown as ReturnType<typeof vi.fn>).mockRejectedValue(new Error('API error'));
    
    const { result } = renderHook(() => useTodoViewModel());
    
    await vi.waitFor(() => {
      expect(result.current.loading).toBe(false);
    });
    
    expect(result.current.error).not.toBeNull();
    expect(result.current.error).toContain('Failed to fetch todos');
  });
  
  it('should add a new todo', async () => {
    const { result } = renderHook(() => useTodoViewModel());
    
    await vi.waitFor(() => {
      expect(result.current.loading).toBe(false);
    });
    
    // Set new todo title
    act(() => {
      result.current.setNewTodoTitle('New Todo Item');
    });
    
    expect(result.current.newTodoTitle).toBe('New Todo Item');
    
    // Add the todo
    await act(async () => {
      await result.current.addTodo();
    });
    
    expect(TodoService.createTodo).toHaveBeenCalledWith({
      title: 'New Todo Item',
      isCompleted: false
    });
    expect(result.current.newTodoTitle).toBe('');
    expect(result.current.todos.length).toBe(3);
  });
  
  it('should not add a todo if the title is empty', async () => {
    const { result } = renderHook(() => useTodoViewModel());
    
    await vi.waitFor(() => {
      expect(result.current.loading).toBe(false);
    });
    
    // Try to add with empty title
    await act(async () => {
      await result.current.addTodo();
    });
    
    expect(TodoService.createTodo).not.toHaveBeenCalled();
  });
  
  it('should toggle todo completion status', async () => {
    const { result } = renderHook(() => useTodoViewModel());
    
    await vi.waitFor(() => {
      expect(result.current.loading).toBe(false);
    });
    
    // Toggle the first todo
    await act(async () => {
      await result.current.toggleTodoCompletion(1);
    });
    
    expect(TodoService.updateTodo).toHaveBeenCalledWith({
      id: 1,
      title: 'Test Todo 1',
      isCompleted: true
    });
    
    // The todo should now be completed in the state
    expect(result.current.todos[0].isCompleted).toBe(true);
  });
  
  it('should delete a todo', async () => {
    const { result } = renderHook(() => useTodoViewModel());
    
    await vi.waitFor(() => {
      expect(result.current.loading).toBe(false);
    });
    
    // Delete the first todo
    await act(async () => {
      await result.current.deleteTodo(1);
    });
    
    expect(TodoService.deleteTodo).toHaveBeenCalledWith(1);
    
    // The todo should be removed from the state
    expect(result.current.todos.length).toBe(1);
    expect(result.current.todos[0].id).toBe(2);
  });
}); 