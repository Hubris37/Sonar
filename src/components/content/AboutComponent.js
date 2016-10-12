'use strict';

import React from 'react';
import Slider from 'react-slick'

require('styles/content/About.css')

let groupMembers = [{
  name: 'Fredrik Berglund',
  email: 'fberglund@kth.se',
  image:require('../../images/member-photos/fredrik-berglund.jpg'),
  contributions:[
    'Shaders',
    'Microphone interaction',
    'Wiimote interaction',
  ]
}, {
  name: 'Karl Andersson',
  email: 'karl9@kth.se',
  image:require('../../images/member-photos/karl-andersson.jpg'),
  contributions:[
    'Modelling',
    'Animation',
    'Natural Leader',
  ]
}, {
  name: 'Karl Gylleus',
  email: 'gylleus@kth.se',
  image:require('../../images/member-photos/karl-gylleus.jpg'),
  contributions:[
    'Modelling',
    'Animation',
    'AI',
  ]
}, {
  name: 'Marcus Ahlström',
  email: 'mahlst@kth.se',
  image:require('../../images/member-photos/marcus-ahlstroem.jpg'),
  contributions:[
    'Procedural maze generation',
    'Microphone interaction',
    'Controller interaction',
    'Interactive objects',
  ]
}, {
  name: 'Rodrigo Roa Rodríguez',
  email: 'rorr@kth.se',
  image:require('../../images/member-photos/rodrigo-roa-rodriguez.jpg'),
  contributions:[
    'Web demo',
    'Web page',
  ]
}, {
  name: 'Staffan Sandberg',
  email: 'stsand@kth.se',
  image:require('../../images/member-photos/staffan-sanberg.jpg'),
  contributions:[
    'Jack of all trades',
    'Morale booster',
    'UI',
    'Tutorial',
    'Cleaner',
  ]
}]

// TODO: add contributions
const memberTemplate  = (member) => (
  <div className="member" key={member.name}>
    <p className='name'>{member.name}</p>
    <img src={member.image} alt={member.name} className="member-image"/>
    <p className='email'><i class="material-icons">email</i>{member.email}</p>
    <p className='contributions'>{}</p>
  </div>
)

class AboutComponent extends React.Component {

  render() {
    let settings = {
      dots: true,
      adaptiveHeight: true,
      swipeToSlide: true,
      speed: 500,
      slidesToScroll: 1,
      slidesToShow : 1
    }

    return (
      <div className="about-component">
          <div className="carousel-wrapper">
            <Slider {...settings}>
            {groupMembers.map(memberTemplate)}
            </Slider>
          </div>
      </div>
    );
    // return (
    //   <div className="about-component">
    //   </div>
    // );
  }
}

AboutComponent.displayName = 'ContentAboutComponent';

// Uncomment properties you need
// AboutComponent.propTypes = {};
// AboutComponent.defaultProps = {};

export default AboutComponent;
