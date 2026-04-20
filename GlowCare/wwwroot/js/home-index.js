var map = '';
        var center;

        function initialize() {
            var mapOptions = {
                zoom: 13,
                center: new google.maps.LatLng(-23.013104, -43.394365),
                scrollwheel: false
            };

            map = new google.maps.Map(document.getElementById('google-map'), mapOptions);

            google.maps.event.addDomListener(map, 'idle', function () {
                calculateCenter();
            });

            google.maps.event.addDomListener(window, 'resize', function () {
                map.setCenter(center);
            });
        }

        function calculateCenter() {
            center = map.getCenter();
        }

        function setServicesCarousel() {
            const $carousel = $('.tm-services-carousel');

            if (!$carousel.length) {
                return;
            }

            if ($carousel.hasClass('slick-initialized')) {
                $carousel.slick('unslick');
            }

            $carousel.slick({
                infinite: true,
                autoplay: true,
                autoplaySpeed: 2500,
                speed: 600,
                arrows: true,
                dots: true,
                slidesToShow: 3,
                slidesToScroll: 3,
                adaptiveHeight: false,
                responsive: [
                    {
                        breakpoint: 992,
                        settings: {
                            slidesToShow: 2
                        }
                    },
                    {
                        breakpoint: 576,
                        settings: {
                            slidesToShow: 1
                        }
                    }
                ]
            });
        }



        function setPageNav() {
            if ($(window).width() > 991) {
                $('#tm-top-bar').singlePageNav({
                    currentClass: 'active',
                    offset: 79
                });
            }
            else {
                $('#tm-top-bar').singlePageNav({
                    currentClass: 'active',
                    offset: 65
                });
            }
        }


        $(document).ready(function () {
            $(window).on("scroll", function () {
                if ($(window).scrollTop() > 100) {
                    $(".tm-top-bar").addClass("active");
                } else {
                    $(".tm-top-bar").removeClass("active");
                }
            });

            setPageNav();
            setServicesCarousel();

            $(window).resize(function () {
                setPageNav();
                setServicesCarousel();
            });

            $('.nav-link').click(function () {
                $('#mainNav').removeClass('show');
            });

            $('.tm-btn-play').click(function () {
                togglePlayPause();
            });

            $('.tm-btn-pause').click(function () {
                togglePlayPause();
            });

            $('.tm-current-year').text(new Date().getFullYear());
        });

const checkAvailabilityButton = document.getElementById("checkAvailabilityButton");
        const dateInput = document.getElementById("dateInput");
        const timeInput = document.getElementById("timeInput");

        if (checkAvailabilityButton) {
            checkAvailabilityButton.addEventListener("click", function () {
                var employeeId = document.getElementById("employeeSelect").value;
                var serviceId = document.getElementById("serviceSelect").value;
                var date = document.getElementById("dateInput").value;
                var time = document.getElementById("timeInput").value;

                if (!employeeId || !serviceId || !date || !time) {
                    alert("Моля, попълнете всички полета!");
                    return;
                }

                var dateTime = date + "T" + time;
                document.getElementById("appointmentDateHidden").value = dateTime;

                fetch(`/Procedure/CheckAvailability?employeeId=${employeeId}&serviceId=${serviceId}&dateTime=${encodeURIComponent(dateTime)}`, {
                    credentials: "same-origin"
                })
                    .then(response => {
                        if (response.redirected) {
                            window.location.href = "/Identity/Account/Login?returnUrl=" + encodeURIComponent("/Home/Index");
                            return null;
                        }

                        return response.text();
                    })
                    .then(html => {
                        if (!html) {
                            return;
                        }

                        var modal = document.getElementById("availabilityModal");
                        modal.innerHTML = html;
                        modal.style.display = "flex";
                    })
                    .catch(error => {
                        console.error(error);
                        alert("Възникна грешка при проверката.");
                    });
            });
        }

        function updateAppointmentDate() {
            var date = document.getElementById("dateInput").value;
            var time = document.getElementById("timeInput").value;

            if (date && time) {
                document.getElementById("appointmentDateHidden").value = date + "T" + time;
            }
        }

        if (dateInput) {
            dateInput.addEventListener("change", updateAppointmentDate);
        }

        if (timeInput) {
            timeInput.addEventListener("change", updateAppointmentDate);
        }

        function enablePicker(inputId) {
            var input = document.getElementById(inputId);

            if (!input) {
                return;
            }

            input.addEventListener("click", function () {
                if (this.showPicker) {
                    this.showPicker();
                }
            });

            input.addEventListener("focus", function () {
                if (this.showPicker) {
                    this.showPicker();
                }
            });
        }

        enablePicker("dateInput");
        enablePicker("timeInput");

        document.addEventListener("click", function (e) {
            if (e.target && e.target.id === "cancelButton") {
                document.getElementById("availabilityModal").style.display = "none";
            }

            if (e.target && e.target.id === "confirmBookButton") {
                document.getElementById("procedureForm").submit();
            }
        });

const employeeSelect = document.getElementById("employeeSelect");

        if (employeeSelect) {
            employeeSelect.addEventListener("change", function () {
                var employeeId = this.value;
                var serviceDropdown = document.getElementById("serviceSelect");

                serviceDropdown.innerHTML = '<option value="">Процедура</option>';

                if (!employeeId) return;

                fetch(`/Procedure/GetServicesByEmployee?employeeId=${employeeId}`)
                    .then(response => response.json())
                    .then(data => {
                        data.forEach(service => {
                            var option = document.createElement("option");
                            option.value = service.value;
                            option.text = service.text;
                            serviceDropdown.appendChild(option);
                        });
                    })
                    .catch(error => {
                        console.error("Error:", error);
                    });
            });
        }

document.addEventListener("DOMContentLoaded", function () {
            const chooseButtons = document.querySelectorAll(".choose-service-btn");

            chooseButtons.forEach(button => {
                button.addEventListener("click", function () {
                    const bookingSection = document.getElementById("top");
                    if (bookingSection) {
                        bookingSection.scrollIntoView({ behavior: "smooth" });
                    }
                });
            });
        });
