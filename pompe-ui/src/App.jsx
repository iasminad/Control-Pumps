useEffect(() => {
  fetch("https://localhost:5040/api/pompe/presiune")
    .then(res => res.json())
    .then(data => {
      console.log("Date primite:", data);
     
    });
}, []);
