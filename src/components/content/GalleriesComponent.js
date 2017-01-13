import React from 'react'
import ImageGallery from 'react-image-gallery'
import 'react-image-gallery/styles/css/image-gallery.css'

// General code
const getimagePaths = (imagePaths, imageFileNames) => imagePaths.map((path, i) => ({
  original: path,
  thumbnail: path,
  originalAlt: 'original ' + imageFileNames[i],
  thumbnailAlt: 'thumbnail ' + imageFileNames[i]
}))

class Gallery extends React.Component {
  render() {
    return (
      <ImageGallery
        ref={i => this._imageGallery=i}
        items={this.props.images}
        slideInterval={3000}
        lazyLoading = {true}
      />
    );
  }
}

// Code specific to each Gallery.

// Forskarfredag
const forskarfredagFileNames = [
  '4Hubris1Player.jpg',
  'Marcus+Bystander.jpg',
  'Marcus+Girl.jpg',
  'MarcusHelping.jpg',
  'MarcusRodrigoPlayerBoy.jpg',
  'StrangerDanger.jpg'
]

const forskarfredagPaths = forskarfredagFileNames.map(filename => require('../../images/forskar-fredag/' + filename))

const Forskarfredag = () => <Gallery images={getimagePaths(forskarfredagPaths, forskarfredagFileNames)} />

// Comic-con
const comicConFileNames = [
  'PB040474.JPG',
  'PB040477.JPG',
  'PB040492.JPG',
  'PB040493.JPG',
  'PB040517.JPG',
  'PB040518.JPG',
  'PB040519.JPG',
  'PB040546.JPG'
]

const comicConPaths = comicConFileNames.map(filename => require('../../images/comic-con/' + filename))

const ComicCon = () => <Gallery images={getimagePaths(comicConPaths, comicConFileNames)} />

// Mediabranchdagen
const branchdagenFileNames = [
  'IMG_3099.JPG',
  'IMG_3100.JPG',
  'IMG_3101.JPG'
]

const branchdagenPaths = branchdagenFileNames.map(filename => require('../../images/media-branchdagen/' + filename))

const Branchdagen = () => <Gallery images={getimagePaths(branchdagenPaths, branchdagenFileNames)} />

// Wrap everything in a component
const AllGalleries = () => <div>
  <h2>Forskarfredag</h2>
  <Forskarfredag />
  <h2>Comic-Con</h2>
  <ComicCon />
  <h2>Mediabranchdagen</h2>
  <Branchdagen />
</div>


export default AllGalleries;
