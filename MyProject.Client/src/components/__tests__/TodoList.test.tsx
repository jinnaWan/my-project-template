import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { TodoList } from '../TodoList';
import { useTodoViewModel } from '../../hooks/useTodoViewModel';

// Mock the hook
vi.mock('../../hooks/useTodoViewModel');

describe('TodoList', () => {
  const mockUseTodoViewModel = useTodoViewModel as jest.Mock;
  
  const mockTodos = [
    { id: 1, title: 'Test Todo 1', isCompleted: false },
    { id: 2, title: 'Test Todo 2', isCompleted: true }
  ];
  
  const mockSetNewTodoTitle = vi.fn();
  const mockAddTodo = vi.fn();
  const mockToggleTodoCompletion = vi.fn();
  const mockDeleteTodo = vi.fn();
  
  beforeEach(() => {
    mockUseTodoViewModel.mockReturnValue({
      todos: mockTodos,
      loading: false,
      error: null,
      newTodoTitle: 'New Todo',
      setNewTodoTitle: mockSetNewTodoTitle,
      addTodo: mockAddTodo,
      toggleTodoCompletion: mockToggleTodoCompletion,
      deleteTodo: mockDeleteTodo
    });
  });
  
  it('renders the todo list with items', () => {
    render(<TodoList />);
    
    expect(screen.getByText('Todo List')).toBeInTheDocument();
    expect(screen.getByText('Test Todo 1')).toBeInTheDocument();
    expect(screen.getByText('Test Todo 2')).toBeInTheDocument();
  });
  
  it('shows loading state when loading is true', () => {
    mockUseTodoViewModel.mockReturnValue({
      todos: [],
      loading: true,
      error: null,
      newTodoTitle: '',
      setNewTodoTitle: mockSetNewTodoTitle,
      addTodo: mockAddTodo,
      toggleTodoCompletion: mockToggleTodoCompletion,
      deleteTodo: mockDeleteTodo
    });
    
    render(<TodoList />);
    
    expect(screen.getByText('Loading todos...')).toBeInTheDocument();
  });
  
  it('shows error message when there is an error', () => {
    mockUseTodoViewModel.mockReturnValue({
      todos: [],
      loading: false,
      error: 'Failed to fetch todos',
      newTodoTitle: '',
      setNewTodoTitle: mockSetNewTodoTitle,
      addTodo: mockAddTodo,
      toggleTodoCompletion: mockToggleTodoCompletion,
      deleteTodo: mockDeleteTodo
    });
    
    render(<TodoList />);
    
    expect(screen.getByText('Failed to fetch todos')).toBeInTheDocument();
  });
  
  it('calls addTodo when Add button is clicked', () => {
    render(<TodoList />);
    
    const addButton = screen.getByText('Add');
    fireEvent.click(addButton);
    
    expect(mockAddTodo).toHaveBeenCalledTimes(1);
  });
  
  it('updates newTodoTitle when input changes', () => {
    render(<TodoList />);
    
    const input = screen.getByPlaceholderText('Add new todo');
    fireEvent.change(input, { target: { value: 'New Todo Item' } });
    
    expect(mockSetNewTodoTitle).toHaveBeenCalledWith('New Todo Item');
  });
  
  it('calls toggleTodoCompletion when todo is clicked', () => {
    render(<TodoList />);
    
    const todoItem = screen.getByText('Test Todo 1');
    fireEvent.click(todoItem);
    
    expect(mockToggleTodoCompletion).toHaveBeenCalledWith(1);
  });
  
  it('calls deleteTodo when Delete button is clicked', () => {
    render(<TodoList />);
    
    const deleteButtons = screen.getAllByText('Delete');
    fireEvent.click(deleteButtons[0]);
    
    expect(mockDeleteTodo).toHaveBeenCalledWith(1);
  });
}); 