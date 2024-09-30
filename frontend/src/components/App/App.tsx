import { createGlobalStyle } from 'styled-components';
import DutchScoresTable from '../DutchScoreTable/DutchScoreTable'
import { Header } from '../Header/Header'

const GlobalStyle = createGlobalStyle`
  body {
    margin: 0;
    background-color: #2d2d2d; 
    color: #f5f5f5;
    font-family: 'Roboto', sans-serif;
  }
`;

function App() {
  return (
    <>
      <GlobalStyle/>
      <div id="container">
        <Header></Header>
        <DutchScoresTable></DutchScoresTable>
      </div>
    </>
  )
}

export default App
