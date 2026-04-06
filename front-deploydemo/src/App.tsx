 
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import './App.css'
import Demo from './components/demo'
import Login from './components/Login';
import { ProtectedRoute } from './common-components/protectroute';

function App() {
   
  return (     
    <BrowserRouter> 
      <Routes>
        <Route path="/" element={<Login />} /> 
        <Route element={<ProtectedRoute />}>
          <Route path="/demo" element={<Demo />} />
        </Route>
      </Routes>
    </BrowserRouter>
  ); 
}

export default App
