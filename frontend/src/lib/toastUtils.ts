import { toast } from "sonner";
import axios from "axios";

export const toastUtils = {
  // Success
  success: (message: string, description?: string) => {
    toast.success(message, { description });
  },

  // Error with axios support
  error: (message: string, description?: string | Error) => {
    let errorMessage = message;
    let errorDescription = description;

    if (description instanceof Error) {
      errorDescription = description.message;
    }

    toast.error(errorMessage, { description: String(errorDescription) });
  },

  // API Error handler
  apiError: (error: unknown) => {
    if (axios.isAxiosError(error)) {
      const message = error.response?.data?.message || error.message || "An error occurred";
      const status = error.response?.status;
      toast.error("Error", {
        description: `${status ? `[${status}] ` : ""}${message}`,
      });
    } else if (error instanceof Error) {
      toast.error("Error", { description: error.message });
    } else {
      toast.error("An unexpected error occurred");
    }
  },

  // Info
  info: (message: string, description?: string) => {
    toast.info(message, { description });
  },

  // Warning
  warning: (message: string, description?: string) => {
    toast.warning(message, { description });
  },

  // Loading with promise
  loading: (message: string, description?: string) => {
    return toast.loading(message, { description });
  },

  // Promise-based toast
  promise: <T,>(
    promise: Promise<T>,
    messages: {
      loading: string;
      success?: string;
      error?: string;
    }
  ) => {
    toast.promise(promise, {
      loading: messages.loading,
      success: messages.success || "Success!",
      error: messages.error || "Something went wrong",
    });
  },

  // Dismiss a specific toast
  dismiss: (toastId: string | number) => {
    toast.dismiss(toastId);
  },
};
