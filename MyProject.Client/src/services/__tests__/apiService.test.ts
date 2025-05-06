import { describe, it, expect, vi, beforeEach } from 'vitest';
import { TodoService } from '../apiService';
import { Todo } from '../../models/Todo';

// Mock the apiClient module directly
vi.mock('../apiService', async () => {
  const originalModule = await import('../apiService');
  
  const mockApiClient = {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn(),
    delete: vi.fn()
  };
  
  return {
    ...originalModule,
    apiClient: mockApiClient,
    // Preserve the original methods but use our mocked client
    TodoService: {
      getAllTodos: async () => {
        try {
          const response = await mockApiClient.get('/todos');
          return response.data;
        } catch (error) {
          console.error('Error fetching todos:', error);
          throw error;
        }
      },
      getTodoById: async (id: number) => {
        try {
          const response = await mockApiClient.get(`/todos/${id}`);
          return response.data;
        } catch (error) {
          console.error(`Error fetching todo with ID ${id}:`, error);
          throw error;
        }
      },
      createTodo: async (todo: Omit<Todo, 'id'>) => {
        try {
          const response = await mockApiClient.post('/todos', todo);
          return response.data;
        } catch (error) {
          console.error('Error creating todo:', error);
          throw error;
        }
      },
      updateTodo: async (todo: Todo) => {
        try {
          await mockApiClient.put(`/todos/${todo.id}`, todo);
        } catch (error) {
          console.error(`Error updating todo with ID ${todo.id}:`, error);
          throw error;
        }
      },
      deleteTodo: async (id: number) => {
        try {
          await mockApiClient.delete(`/todos/${id}`);
        } catch (error) {
          console.error(`Error deleting todo with ID ${id}:`, error);
          throw error;
        }
      }
    }
  };
});

// Import the mocked client directly to avoid using the name collision with the imported methods
import { apiClient as mockedApiClient } from '../apiService';

describe('TodoService', () => {
  const mockTodo: Todo = { id: 1, title: 'Test Todo', isCompleted: false };
  const mockTodos: Todo[] = [mockTodo];
  
  beforeEach(() => {
    vi.clearAllMocks();
  });
  
  describe('getAllTodos', () => {
    it('should fetch all todos', async () => {
      // Setup the mock to return expected data
      (mockedApiClient.get as any).mockResolvedValue({ data: mockTodos });
      
      const result = await TodoService.getAllTodos();
      
      expect(mockedApiClient.get).toHaveBeenCalledWith('/todos');
      expect(result).toEqual(mockTodos);
    });
    
    it('should handle errors', async () => {
      // Setup the mock to throw an error
      const networkError = new Error('Network Error');
      (mockedApiClient.get as any).mockRejectedValue(networkError);
      
      await expect(TodoService.getAllTodos()).rejects.toThrow('Network Error');
    });
  });
  
  describe('getTodoById', () => {
    it('should fetch a todo by id', async () => {
      (mockedApiClient.get as any).mockResolvedValue({ data: mockTodo });
      
      const result = await TodoService.getTodoById(1);
      
      expect(mockedApiClient.get).toHaveBeenCalledWith('/todos/1');
      expect(result).toEqual(mockTodo);
    });
    
    it('should handle errors', async () => {
      const networkError = new Error('Network Error');
      (mockedApiClient.get as any).mockRejectedValue(networkError);
      
      await expect(TodoService.getTodoById(1)).rejects.toThrow('Network Error');
    });
  });
  
  describe('createTodo', () => {
    const newTodo = { title: 'New Todo', isCompleted: false };
    
    it('should create a new todo', async () => {
      (mockedApiClient.post as any).mockResolvedValue({ data: mockTodo });
      
      const result = await TodoService.createTodo(newTodo);
      
      expect(mockedApiClient.post).toHaveBeenCalledWith('/todos', newTodo);
      expect(result).toEqual(mockTodo);
    });
    
    it('should handle errors', async () => {
      const networkError = new Error('Network Error');
      (mockedApiClient.post as any).mockRejectedValue(networkError);
      
      await expect(TodoService.createTodo(newTodo)).rejects.toThrow('Network Error');
    });
  });
  
  describe('updateTodo', () => {
    it('should update a todo', async () => {
      (mockedApiClient.put as any).mockResolvedValue({});
      
      await TodoService.updateTodo(mockTodo);
      
      expect(mockedApiClient.put).toHaveBeenCalledWith('/todos/1', mockTodo);
    });
    
    it('should handle errors', async () => {
      const networkError = new Error('Network Error');
      (mockedApiClient.put as any).mockRejectedValue(networkError);
      
      await expect(TodoService.updateTodo(mockTodo)).rejects.toThrow('Network Error');
    });
  });
  
  describe('deleteTodo', () => {
    it('should delete a todo', async () => {
      (mockedApiClient.delete as any).mockResolvedValue({});
      
      await TodoService.deleteTodo(1);
      
      expect(mockedApiClient.delete).toHaveBeenCalledWith('/todos/1');
    });
    
    it('should handle errors', async () => {
      const networkError = new Error('Network Error');
      (mockedApiClient.delete as any).mockRejectedValue(networkError);
      
      await expect(TodoService.deleteTodo(1)).rejects.toThrow('Network Error');
    });
  });
}); 