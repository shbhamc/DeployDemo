import axios from "axios";

// 1️⃣ Create axios instance (recommended)
export const api = axios.create({
  baseURL: import.meta.env.VITE_BASE_API_URL, // must be string URL
  headers: {
    "Content-Type": "application/json"
  }
});

// 2️⃣ Get tasks
export const getTasks = async () => {
  const token = localStorage.getItem("token"); // add JWT if needed

  const res = await api.get("tasks", {
    headers: {
      Authorization: token ? `Bearer ${token}` : ""
    }
  });

  console.log(res.data);
};

// 3️⃣ Login and store token
export const logins = async (param: { username?: string; password?: string }) => {
  const res = await api.post("Deploy/login", {
    username: param?.username,
    password: param?.password
  });

  // Save JWT to localStorage
  localStorage.setItem("token", res.data.token);
  return res;
};

export const logout = () => {
  localStorage.removeItem("token");
};