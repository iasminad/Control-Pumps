import React from "react";

const SensorIndicator = ({ name, isActive }) => {
  return (
    <div style={{ display: "flex", alignItems: "center", gap: "10px", marginBottom: "10px" }}>
      <div style={{
        width: "20px",
        height: "20px",
        backgroundColor: isActive ? "#f44336" : "#bbb",
        borderRadius: "50%",
        border: "2px solid #333"
      }} />
      <span>{name}</span>
    </div>
  );
};

export default SensorIndicator;
