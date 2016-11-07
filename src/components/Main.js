require('normalize.css/normalize.css');
require('styles/App.css');

import React from 'react';
import HeaderComponent from './layout/HeaderComponent'
import FooterComponent from './layout/FooterComponent'
import AboutComponent from './content/AboutComponent'
import {Introduction, Technology, Design} from './sections'


let components = {
  'Introduction': <Introduction/>,
  'Technology': <Technology/>,
  'Design': <Design/>,
  'About': <AboutComponent/>
}

let sections = Object.keys(components).map(section => ({
  content: section,
  id: '#' + section
}))

const sectionTemplate = section => <div className='section'>
  <h1 key={sections.id}>
    <a name={section.content}>{section.content}</a>
  </h1>
  <div className='section-content'>
    {components[section.content]}
  </div>
</div>

class AppComponent extends React.Component {
  render() {
    return (
      <div className="index">
        <HeaderComponent items={sections}/>
        <main>
        {sections.map(sectionTemplate)}
        </main>
        <FooterComponent/>
      </div>
    );
  }
}

AppComponent.defaultProps = {};

export default AppComponent;
