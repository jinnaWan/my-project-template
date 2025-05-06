import axios, { AxiosResponse } from 'axios';
import { Todo } from '../models/Todo';

// Create an axios instance with default config
const apiClient = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json'
  }
});

/**
 * Service for handling Todo API operations
 */
export const TodoService = {
  /**
   * Gets all Todo items
   * @returns A promise that resolves to an array of Todo items
   */
  async getAllTodos(): Promise<Todo[]> {
    try {
      const response: AxiosResponse<Todo[]> = await apiClient.get('/todos');
      return response.data;
    } catch (error) {
      console.error('Error fetching todos:', error);
      throw error;
    }
  },

  /**
   * Gets a Todo item by its identifier
   * @param id - The Todo item identifier
   * @returns A promise that resolves to the Todo item
   */
  async getTodoById(id: number): Promise<Todo> {
    try {
      const response: AxiosResponse<Todo> = await apiClient.get(`/todos/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Error fetching todo with ID ${id}:`, error);
      throw error;
    }
  },

  /**
   * Creates a new Todo item
   * @param todo - The Todo item to create
   * @returns A promise that resolves to the created Todo item
   */
  async createTodo(todo: Omit<Todo, 'id'>): Promise<Todo> {
    try {
      const response: AxiosResponse<Todo> = await apiClient.post('/todos', todo);
      return response.data;
    } catch (error) {
      console.error('Error creating todo:', error);
      throw error;
    }
  },

  /**
   * Updates an existing Todo item
   * @param todo - The Todo item to update
   * @returns A promise that resolves when the update is complete
   */
  async updateTodo(todo: Todo): Promise<void> {
    try {
      await apiClient.put(`/todos/${todo.id}`, todo);
    } catch (error) {
      console.error(`Error updating todo with ID ${todo.id}:`, error);
      throw error;
    }
  },

  /**
   * Deletes a Todo item
   * @param id - The identifier of the Todo item to delete
   * @returns A promise that resolves when the deletion is complete
   */
  async deleteTodo(id: number): Promise<void> {
    try {
      await apiClient.delete(`/todos/${id}`);
    } catch (error) {
      console.error(`Error deleting todo with ID ${id}:`, error);
      throw error;
    }
  }
}; 