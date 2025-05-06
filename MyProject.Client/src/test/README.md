# Testing Guide

This project uses Vitest for unit testing. The test files are organized alongside the application files in `__tests__` directories.

## Running Tests

To run the tests, use the following commands:

```bash
# Run tests in watch mode (default)
npm test

# Run tests once with coverage report
npm run test:coverage

# Run tests with UI
npm run test:ui
```

## Test Structure

The tests are organized as follows:

- `src/components/__tests__/`: Tests for React components
- `src/hooks/__tests__/`: Tests for custom hooks
- `src/services/__tests__/`: Tests for API services

## Writing Tests

### Component Tests

Component tests use `@testing-library/react` for rendering and interacting with components. Example:

```tsx
import { render, screen, fireEvent } from '@testing-library/react';
import { YourComponent } from '../YourComponent';

describe('YourComponent', () => {
  it('should render correctly', () => {
    render(<YourComponent />);
    
    expect(screen.getByText('Some Text')).toBeInTheDocument();
  });
});
```

### Hook Tests

Hook tests use `@testing-library/react` with the `renderHook` function to test custom hooks. Example:

```tsx
import { renderHook, act } from '@testing-library/react';
import { useYourHook } from '../useYourHook';

describe('useYourHook', () => {
  it('should initialize with default values', () => {
    const { result } = renderHook(() => useYourHook());
    
    expect(result.current.value).toBe(defaultValue);
  });
});
```

### Service Tests

Service tests mock external dependencies (like `axios`) and test the service functions. Example:

```tsx
import { vi } from 'vitest';
import axios from 'axios';
import { YourService } from '../yourService';

vi.mock('axios');

describe('YourService', () => {
  it('should fetch data correctly', async () => {
    (axios.get as any).mockResolvedValue({ data: mockData });
    
    const result = await YourService.getData();
    
    expect(result).toEqual(mockData);
  });
});
```

## Mocking

The tests use Vitest's mocking capabilities. To mock a module:

```tsx
import { vi } from 'vitest';

vi.mock('path/to/module', () => ({
  someFunction: vi.fn(),
  someOtherFunction: vi.fn()
}));
``` 