class Theatre:
    def __init__(self, name, location, capacity, genres, description, director_bio, critic_reviews):
        self.name = name  # keyword
        self.location = location  #  keyword
        self.capacity = capacity  #  integer
        self.genres = genres  # keywords
        self.description = description  # text
        self.director_bio = director_bio  # text
        self.critic_reviews = critic_reviews  # text

    def to_dict(self):
        return {
            "name": self.name,
            "location": self.location,
            "capacity": self.capacity,
            "genres": self.genres,
            "description": self.description,
            "director_bio": self.director_bio,
            "critic_reviews": self.critic_reviews
        }
