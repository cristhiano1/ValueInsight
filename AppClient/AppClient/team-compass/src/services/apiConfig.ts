// API Configuration
// En Docker usamos URL directa al backend
// En local puedes usar proxy (/api)

export const API_BASE_URL =
    import.meta.env.VITE_API_BASE_URL || "http://backend:8080/api";

export async function apiFetch<T>(
    endpoint: string,
    options?: RequestInit
): Promise<T> {
    const url = `${API_BASE_URL}${endpoint}`;

    const response = await fetch(url, {
        headers: {
            "Content-Type": "application/json",
            ...options?.headers,
        },
        ...options,
    });

    if (!response.ok) {
        throw new Error(`API Error: ${response.status} ${response.statusText}`);
    }

    return response.json();
}