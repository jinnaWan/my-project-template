import { afterEach, vi } from 'vitest';
import '@testing-library/jest-dom';
import { cleanup } from '@testing-library/react';

// Automatically clean up after each test
afterEach(() => {
  // Clean up any React testing-library rendered components
  cleanup();
  
  // Reset any mocked modules after each test
  vi.restoreAllMocks();
}); 