import React from 'react'
import ImageGallery from 'react-image-gallery'
import 'react-image-gallery/styles/css/image-gallery.css'


const imageNames = [
  '4Hubris1Player.jpg',
  'Marcus+Bystander.jpg',
  'Marcus+Girl.jpg',
  'Marcus+Girl2.jpg',
  'MarcusHelping.jpg',
  'MarcusRodrigoPlayerBoy.jpg',
  'StrangerDanger.jpg'
]
//Development path
const path = '../../images/ForskarFredag/'

//Distribution path
// const path = 'https://hubris37.github.io/Sonar/src/images/forskarFredag/'

const imagePaths = imageNames.map(filename =>path+filename)

const images = imagePaths.map( (path, i) => ({
    original: path,
    thumbnail: path,
    originalAlt: 'original '+imageNames[i],
    thumbnailAlt: 'thumbnail '+imageNames[i]
  }))



class ForskarFredag extends React.Component {
  render() {
    return (
      <ImageGallery
        ref={i => this._imageGallery = i}
        items={images}
        slideInterval={3000}
        lazyLoading = {true}
      />
    );
  }

}

export default ForskarFredag;
