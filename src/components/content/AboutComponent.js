'use strict';

import React from 'react';
import Slider from 'react-slick'

require('styles/content/About.css')

let groupMembers = [{
  name: 'Fredrik Berglund',
  email: 'fberglund@kth.se',
  image:require('../../images/member-photos/fredrik-berglund.jpg')
}, {
  name: 'Karl Andersson',
  email: 'karl9@kth.se',
  image:require('../../images/member-photos/karl-andersson.jpg')
}, {
  name: 'Karl Gylleus',
  email: 'gylleus@kth.se',
  image:require('../../images/member-photos/karl-gylleus.jpg')
}, {
  name: 'Marcus Ahlström',
  email: 'mahlst@kth.se',
  image:require('../../images/member-photos/marcus-ahlstroem.jpg')
}, {
  name: 'Rodrigo Roa Rodríguez',
  email: 'rorr@kth.se',
  image:require('../../images/member-photos/rodrigo-roa-rodriguez.jpg')
}, {
  name: 'Staffan Sandberg',
  email: 'stsand@kth.se',
  image:require('../../images/member-photos/staffan-sanberg.jpg')
}]

const memberTemplate = member => <div className="member" key={member.name}>
  <img src={member.image} alt={member.name} className="member-image"/>
  <p className='name'>{member.name}</p>
  <p className='email'>{member.email}</p>
  </div>

class AboutComponent extends React.Component {

  render() {
    let settings = {
      dots: true,
      adaptiveHeight: true,
      swipeToSlide: true,
      speed: 500,
      slidesToShow: 3,
      slidesToScroll: 1
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
