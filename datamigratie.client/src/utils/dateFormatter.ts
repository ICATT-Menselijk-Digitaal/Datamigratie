/**
 * Formats a date string to Dutch locale date-time format
 * @param dateString - ISO date string or null
 * @returns Formatted date string in format "DD-MM-YYYY HH:MM" or "-" if null
 * @example
 * formatDateTime("2024-01-29T14:30:00Z") // "29-01-2024 14:30"
 * formatDateTime(null) // "-"
 */
export function formatDateTime(dateString: string | null): string {
  if (!dateString) return "-";
  
  const date = new Date(dateString);
  
  return date.toLocaleString("nl-NL", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit"
  });
}

/**
 * Formats a date string to Dutch locale date format (without time)
 * @param dateString - ISO date string or null
 * @returns Formatted date string in format "DD-MM-YYYY" or "-" if null
 * @example
 * formatDate("2024-01-29T14:30:00Z") // "29-01-2024"
 * formatDate(null) // "-"
 */
export function formatDate(dateString: string | null): string {
  if (!dateString) return "-";
  
  const date = new Date(dateString);
  
  return date.toLocaleString("nl-NL", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit"
  });
}

/**
 * Formats a date string to Dutch locale time format (without date)
 * @param dateString - ISO date string or null
 * @returns Formatted time string in format "HH:MM" or "-" if null
 * @example
 * formatTime("2024-01-29T14:30:00Z") // "14:30"
 * formatTime(null) // "-"
 */
export function formatTime(dateString: string | null): string {
  if (!dateString) return "-";
  
  const date = new Date(dateString);
  
  return date.toLocaleString("nl-NL", {
    hour: "2-digit",
    minute: "2-digit"
  });
}
