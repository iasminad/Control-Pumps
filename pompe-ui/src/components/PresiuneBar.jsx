import React, { useState, useEffect } from 'react';
import './PresiuneBar.css';

const PressureBar = () => {
  const [pressure, setPressure] = useState(0);

  useEffect(() => {
    const interval = setInterval(() => {
      setPressure((prev) => {
        let next = prev + (Math.random() * 0.2 - 0.1);
        if (next > 1.0) next = 1.0;
        if (next < 0) next = 0;
        return Math.round(next * 100) / 100;
      });
    }, 500);
    return () => clearInterval(interval);
  }, []);

  const sensor1 = pressure >= 0.7;
  const sensor2 = pressure >= 0.9;

  return (
    <div className="pressure-container">
      <div className="bar" style={{ width: `${pressure * 100}%` }} />
      <div className="text">Presiune: {pressure} bar</div>
      <div className="senzori">
        <div className={`senzor ${sensor1 ? 'on' : ''}`}>Senzor B1</div>
        <div className={`senzor ${sensor2 ? 'on' : ''}`}>Senzor B2</div>
      </div>
    </div>
  );
};

export default PressureBar;
