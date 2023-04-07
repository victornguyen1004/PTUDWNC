import React, { useEffect, useRef } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faPaperPlane,
  faMapMarked,
  faPhone,
  faEnvelope,
  faCode,
} from "@fortawesome/free-solid-svg-icons";
import "./contact.css";
import emailjs from "@emailjs/browser";

const Contact = () => {
  useEffect(() => {
    document.title = "Trang liên hệ";
  }, []);

  const form = useRef();

  const sendEmail = (e) => {
    e.preventDefault();

    emailjs
      .sendForm(
        "service_oqq9cyk",
        "template_q3rzhze",
        form.current,
        "iTUEF4rNsbNAx4nXM"
      )
      .then(
        (result) => {
          alert("Gửi mail thành công");
          console.log(result.text);
        },
        (error) => {
          alert("Gửi mail thất bại");
          console.log(error.text);
        }
      );
    e.target.reset();
  };

  return (
    <div className="p-5">
      <section id="contact-section">
        <h1 className="section-header">Liên hệ</h1>

        <div className="contact-wrapper">
          <form ref={form} onSubmit={sendEmail}>
            <div className="mb-3 fs-3 text-center text-uppercase">
              Gửi ý kiến
            </div>
            <div className="mb-3">
              <div className="d-flex justify-content-between">
                <label htmlFor="email" className="form-label">
                  Email
                </label>
                <span className="text-secondary fst-italic fs-6">
                  (Bắt buộc)
                </span>
              </div>
              <input
                type="email"
                name="email"
                className="form-control"
                id="email"
                required
              />
            </div>
            <div className="mb-3">
              <div className="d-flex justify-content-between">
                <label htmlFor="subject" className="form-label">
                  Chủ đề
                </label>
                <span className="text-secondary fst-italic fs-6">
                  (Bắt buộc)
                </span>
              </div>
              <input
                type="text"
                name="subject"
                className="form-control"
                id="subject"
                required
              />
            </div>
            <div className="mb-3">
              <div className="d-flex justify-content-between">
                <label htmlFor="content" className="form-label">
                  Nội dung
                </label>
                <span className="text-secondary fst-italic fs-6">
                  (Bắt buộc)
                </span>
              </div>
              <textarea
                className="form-control"
                name="content"
                id="content"
                rows="8"
                required
              ></textarea>
            </div>
            <button
              className="btn btn-primary send-button"
              id="submit"
              type="submit"
              value="SEND"
            >
              <div className="alt-send-button">
                <FontAwesomeIcon icon={faPaperPlane} />
                <span class="send-text">Gửi</span>
              </div>
            </button>
          </form>

          <div className="direct-contact-container">
            <ul className="contact-list">
              <li className="list-item">
                <FontAwesomeIcon icon={faMapMarked} />
                <span className="contact-text place">Đà Lạt</span>
              </li>

              <li className="list-item">
                <FontAwesomeIcon icon={faPhone} />
                <span className="contact-text phone">0123.456.789</span>
              </li>

              <li className="list-item">
                <FontAwesomeIcon icon={faEnvelope} />
                <span className="contact-text gmail">
                  <a href="mailto:2014508@dlu.edu.vn" title="Send me an email">
                    2014508@dlu.edu.vn
                  </a>
                </span>
              </li>
            </ul>

            <hr></hr>
            <ul className="social-media-list">
              <li>
                <a href="mailto:2012394@dlu.edu.vn" className="contact-icon">
                  <FontAwesomeIcon icon={faEnvelope} />
                </a>
              </li>
              <li>
                <a
                  href="https://github.com/victornguyen1004"
                  target="_blank"
                  rel="noreferrer"
                  className="contact-icon"
                >
                  <FontAwesomeIcon icon={faCode} />
                </a>
              </li>
            </ul>
            <hr></hr>

            <div className="copyright">&copy; TatBlog</div>
          </div>
        </div>
      </section>

      {/* <iframe
        src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d681.7835211958258!2d108.44565228640165!3d11.956223942437685!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x317112db826dc281%3A0x7a577a72a3694368!2zS2hvYSBDw7RuZyBuZ2jhu4cgVGjDtG5nIHRpbg!5e0!3m2!1svi!2s!4v1658199935564!5m2!1svi!2s"
        className="mt-5 w-100"
        height="512px"
        title="map"
        style={{ border: 0 }}
        allowFullScreen=""
        loading="lazy"
        referrerPolicy="no-referrer-when-downgrade"
      ></iframe> */}
    </div>
  );
};

export default Contact;
