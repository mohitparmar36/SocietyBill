# Toast Notifications Implementation

## Overview
Toast notifications have been implemented using **Sonner**, a lightweight and customizable toast library that works seamlessly with Tailwind CSS.

## Setup

✅ **Already configured:**
- `sonner` package installed
- `Toaster` component added to `main.tsx` with default position (top-right) and close button
- Custom `useToast` hook created for easy usage
- Toast utility functions available for API interactions

## Usage Examples

### 1. Using the `useToast` Hook (Recommended for Components)

```typescript
import { useToast } from "@/hooks/useToast";

const MyComponent = () => {
  const toast = useToast();

  const handleSubmit = async () => {
    toast.success("Success!", "Operation completed successfully");
  };

  const handleError = () => {
    toast.error("Error", "Something went wrong");
  };

  return (
    <div>
      <button onClick={handleSubmit}>Submit</button>
      <button onClick={handleError}>Show Error</button>
    </div>
  );
};
```

### 2. Using Toast Utilities (For Non-Component Code)

```typescript
import { toastUtils } from "@/lib/toastUtils";

// Simple messages
toastUtils.success("Saved!", "Your changes have been saved");
toastUtils.error("Failed!", "Could not save changes");
toastUtils.warning("Warning", "This action cannot be undone");
toastUtils.info("Info", "Check your email for confirmation");

// API error handling
try {
  const response = await api.get("/data");
} catch (error) {
  toastUtils.apiError(error); // Automatically extracts and displays API errors
}
```

### 3. Promise-Based Toasts (For Async Operations)

```typescript
import { useToast } from "@/hooks/useToast";

const MyComponent = () => {
  const toast = useToast();

  const handleAsyncOperation = () => {
    const promise = new Promise((resolve) => {
      setTimeout(() => resolve("Done!"), 2000);
    });

    toast.promise(promise, {
      loading: "Processing...",
      success: "Completed successfully!",
      error: "Failed to process",
    });
  };

  return <button onClick={handleAsyncOperation}>Start Async Task</button>;
};
```

### 4. Loading Toast

```typescript
import { toastUtils } from "@/lib/toastUtils";

const handleUpload = async () => {
  const toastId = toastUtils.loading("Uploading...", "Please wait");
  
  try {
    await uploadFile();
    toastUtils.success("Upload complete!");
  } catch (error) {
    toastUtils.error("Upload failed");
  } finally {
    toastUtils.dismiss(toastId);
  }
};
```

### 5. In API Calls (React Query)

```typescript
import { useMutation } from "@tanstack/react-query";
import { useToast } from "@/hooks/useToast";
import { toastUtils } from "@/lib/toastUtils";

const MyComponent = () => {
  const toast = useToast();

  const mutation = useMutation({
    mutationFn: async (data) => {
      return api.post("/endpoint", data);
    },
    onSuccess: () => {
      toast.success("Success!", "Operation completed");
    },
    onError: (error) => {
      toastUtils.apiError(error);
    },
  });

  return <button onClick={() => mutation.mutate({})}>Submit</button>;
};
```

## Available Toast Methods

### Hook Version (`useToast`)
- `success(message, description?)` - Green toast
- `error(message, description?)` - Red toast
- `info(message, description?)` - Blue toast
- `warning(message, description?)` - Orange toast
- `loading(message, description?)` - Loading spinner
- `promise(promise, messages)` - For async operations

### Utility Version (`toastUtils`)
- `success(message, description?)`
- `error(message, description?)`
- `apiError(error)` - Extracts Axios error messages
- `info(message, description?)`
- `warning(message, description?)`
- `loading(message, description?)` - Returns toast ID
- `promise(promise, messages)`
- `dismiss(toastId)` - Close a specific toast

## Toaster Configuration

The Toaster is configured in `main.tsx` with:
- **Position**: `top-right` (can be changed to: bottom-right, bottom-center, top-center, etc.)
- **Rich Colors**: Enabled (different background colors for each type)
- **Close Button**: Enabled (users can dismiss toasts)

To customize, update `main.tsx`:
```typescript
<Toaster 
  position="bottom-right" 
  richColors 
  closeButton 
  expand={true}
  duration={4000}
/>
```

## Files Created/Modified

1. **Modified**: `src/main.tsx` - Added Toaster provider
2. **Created**: `src/hooks/useToast.ts` - Custom hook for components
3. **Created**: `src/lib/toastUtils.ts` - Utility functions for non-component code

## Next Steps

- Integrate toasts in your API error handlers
- Use in form submissions and async operations
- Customize position/styling in `main.tsx` if needed
