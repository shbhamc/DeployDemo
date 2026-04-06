import { useEffect, useState } from "react";
import { api, logout } from "../services/axios";
import { useNavigate } from "react-router-dom";
const API_URL = api + "Deploy";

type DeployDemo = {
  id: number;
  name: string;
};

function Demo() {
  const [data, setData] = useState<DeployDemo[]>([]);
  const [name, setName] = useState("");
  const [editId, setEditId] = useState<number | null>(null);
  const [searchId, setSearchId] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  // ✅ Load All
  const fetchData = async () => {
    try {
      setLoading(true);
      const res = await api.get<DeployDemo[]>(API_URL);
      setData(res.data);
    } catch {
      setError("Failed to fetch data");
    } finally {
      setLoading(false);
    }
  };

  // ✅ useEffect FIXED
  useEffect(() => {
    const load = async () => {
      await fetchData();
    };
    load();
  }, []);

  // ✅ Get By Id
  const fetchById = async () => {
    if (!searchId) return;
    try {
      const res = await api.get<DeployDemo>(`${API_URL}/${searchId}`);
      setData([res.data]);
    } catch {
      setError("Record not found");
    }
  };

  // ✅ Add
  const addData = async () => {
    if (!name.trim()) return;
    await api.post(API_URL, { name });
    setName("");
    fetchData();
  };

  // ✅ Update
  const updateData = async () => {
    if (!name.trim() || editId === null) return;

    await api.put(`${API_URL}/${editId}`, {
      id: editId,
      name,
    });

    setEditId(null);
    setName("");
    fetchData();
  };

  // ✅ Delete
  const deleteData = async (id: number) => {
    await api.delete(`${API_URL}/${id}`);
    fetchData();
  };

  const startEdit = (item: DeployDemo) => {
    setEditId(item.id);
    setName(item.name);
  };
  const navigate = useNavigate();

  const handleLogout = () => {
    logout(); // remove token
    navigate("/"); // go to login page
  };


  return (
    <div style={{ padding: "30px", fontFamily: "Arial" }}>
      <h2>DeployDemo CRUD</h2>
      <div>
        <button onClick={handleLogout}>
          Logout
        </button>
      </div>

      {error && <p style={{ color: "red" }}>{error}</p>}
      {loading && <p>Loading...</p>}

      {/* Add / Update */}
      <input
        type="text"
        placeholder="Enter Name"
        value={name}
        onChange={(e) => setName(e.target.value)}
      />
      {editId !== null ? (
        <button onClick={updateData}>Update</button>
      ) : (
        <button onClick={addData}>Add</button>
      )}

      <hr />

      {/* Get By Id */}
      <input
        type="number"
        placeholder="Search by Id"
        value={searchId}
        onChange={(e) => setSearchId(e.target.value)}
      />
      <button onClick={fetchById}>Search</button>
      <button onClick={fetchData}>Reset</button>

      <hr />

      {/* List */}
      <table cellPadding="10" border={1}>
        <thead>
          <tr>
            <th>Id</th>
            <th>Name</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {data.map((item) => (
            <tr key={item.id}>
              <td>{item.id}</td>
              <td>{item.name}</td>
              <td>
                <button onClick={() => startEdit(item)}>Edit</button>
                <button onClick={() => deleteData(item.id)}>Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default Demo;