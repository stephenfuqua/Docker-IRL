import { useState, useEffect } from 'react'
import './App.css'

// Todo: pull in URL from config.js
const weatherForecastUrl = 'http://localhost:4173/api/weatherforecast'


function App() {
    const [forecastData, setForecastData] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
      const fetchForecast = async () => {
        try {
          setIsLoading(true);
          const response = await fetch(weatherForecastUrl);
          if (!response.ok) {
            throw new Error('Failed to fetch forecast data');
          }
          const data = await response.json();
          setForecastData(data);
        } catch (err) {
          setError(err.message);
        } finally {
          setIsLoading(false);
        }
      };

      fetchForecast();
    }, []);

    if (isLoading) {
      return <div>Loading...</div>;
    }

    if (error) {
      return <div>Error: {error}</div>;
    }

    return (
        <table className="forecast-container">
            <thead>
                <tr>
                    <th>Date</th>
                    <th>Temperature (C)</th>
                    <th>Temperature (F)</th>
                    <th>Summary</th>
                </tr>
            </thead>
            <tbody>
                {forecastData.map((item, index) => (
                    <tr key={index}>
                        <td>{item.date}</td>
                        <td>{item.temperatureC}</td>
                        <td>{item.temperatureF}</td>
                        <td>{item.summary}</td>
                    </tr>
                ))}
            </tbody>
        </table>
    );
  }

export default App
