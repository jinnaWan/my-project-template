/**
 * Represents a Todo item
 */
export interface Todo {
  /**
   * The unique identifier for the Todo item
   */
  id: number;
  
  /**
   * The title or description of the Todo item
   */
  title: string;
  
  /**
   * Indicates whether the Todo item has been completed
   */
  isCompleted: boolean;
} 